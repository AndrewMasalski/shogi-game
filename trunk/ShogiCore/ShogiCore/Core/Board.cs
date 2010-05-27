using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Yasc.ShogiCore.Moves;
using Yasc.ShogiCore.Primitives;
using Yasc.ShogiCore.Snapshots;
using Yasc.Utils;
using Yasc.Utils.Mvvm;

namespace Yasc.ShogiCore.Core
{
  /// <summary>Represents observable and mutable shogi board with moves tracking, analysis, etc.</summary>
  public class Board : ObservableObject
  {
    // TODO: Sfen strings

    #region ' Fields '

    private int _currentMoveIndex;
    private readonly Flag _moving = new Flag();
    private Player _oneWhoMoves;
    private bool _isMovesOrderMaintained = true;
    private readonly Cell[,] _cells = new Cell[9, 9];
    private BoardSnapshot _currentSnapshot;
    private ShogiGameResult _gameResult;

    #endregion

    #region ' Properties and Indexers '

    /// <summary>Snapshot of the board's current state</summary>
    public BoardSnapshot CurrentSnapshot
    {
      get { return _currentSnapshot ?? (_currentSnapshot = TakeSnapshot()); }
    }
    /// <summary>The one who plays white pieces</summary>
    public Player White { get; private set; }
    /// <summary>The one who plays black pieces</summary>
    public Player Black { get; private set; }
    /// <summary>The player who moves next</summary>
    public Player OneWhoMoves
    {
      get { return _oneWhoMoves; }
      set
      {
        if (_oneWhoMoves == value) return;
        if (value != White && value != Black)
          throw new ArgumentOutOfRangeException("value");
        _oneWhoMoves = value;
        RaisePropertyChanged("OneWhoMoves");
        ResetCurrentSnapshot();
      }
    }
    /// <summary>Whether the board tracks moves order</summary>
    /// <remarks>If board tracks moves order some moves are going to be invalid with message 
    ///   "its white's (black's) move now.</remarks>
    public bool IsMovesOrderMaintained
    {
      get { return _isMovesOrderMaintained; }
      set
      {
        if (_isMovesOrderMaintained == value) return;
        _isMovesOrderMaintained = value;
        RaisePropertyChanged("IsMovesOrderMaintained");
      }
    }
    /// <summary>Moves history</summary>
    /// <remarks>Only contains moves made through <see cref="MakeMove"/>. 
    ///   All other changes made to the board are not counted here.</remarks>
    public MovesHistory History { get; private set; }
    /// <summary>81 cells of the board in stable order</summary>
    public IEnumerable<Cell> Cells
    {
      get { return Position.OnBoard.Select(p => _cells[p.X, p.Y]); }
    }
    /// <summary>Gets piece in cell at  position -or- null if the cell is empty</summary>
    public Piece GetPieceAt(Position position)
    {
      return _cells[position.X, position.Y].Piece;
    }
    /// <summary>Gets cell at position</summary>
    public Cell GetCellAt(Position position)
    {
      return _cells[position.X, position.Y];
    }
    /// <summary>Gets the cell in the position</summary>
    public Cell GetCellAt(int x, int y)
    {
      return _cells[x, y];
    }
    /// <summary>Gets the player of the given color</summary>
    public Player GetPlayer(PieceColor color)
    {
      return color == PieceColor.White ? White : Black;
    }
    /// <summary>Gets the game result (or <see cref="ShogiGameResult.None"/> otherwise)</summary>
    public ShogiGameResult GameResult
    {
      get { return _gameResult; }
      set
      {
        if (_gameResult == value) return;
        _gameResult = value;
        RaisePropertyChanged("GameResult");
      }
    }

    #endregion

    #region ' Ctors, Load '

    /// <summary>ctor</summary>
    public Board(IPieceSet pieceSet)
    {
      if (pieceSet == null) throw new ArgumentNullException("pieceSet");

      PieceSet = pieceSet;
      History = CreateMovesHistory();
      White = CreatePlayer();
      Black = CreatePlayer();

      _oneWhoMoves = Black;

      FillCells();
    }

    /// <summary>Resets the pieces on board and from hands and loads a snapshot from scratch</summary>
    /// <exception cref="NotEnoughPiecesInSetException">When board's piece set has not enough piecese</exception>
    public void LoadSnapshot(BoardSnapshot snapshot)
    {
      if (snapshot == null) throw new ArgumentNullException("snapshot");
      OneWhoMoves = GetPlayer(snapshot.OneWhoMoves);

      ResetAll();

      foreach (var p in Position.OnBoard)
        if (snapshot.GetPieceAt(p) != null)
        {
          var piece = PieceSet[snapshot.GetPieceAt(p).PieceType];
          if (piece == null)
          {
            throw new NotEnoughPiecesInSetException(
              "Can't load snapshot because it's not enough pieces in set: " +
              "couldn't find " + snapshot.GetPieceAt(p).PieceType + " to fill " + p);
          }
          SetPiece(piece, p, GetPlayer(snapshot.GetPieceAt(p).Color));
        }

      White.Hand.LoadSnapshot(snapshot.WhiteHand);
      Black.Hand.LoadSnapshot(snapshot.BlackHand);
    }

    private void FillCells()
    {
      foreach (var p in Position.OnBoard)
        _cells[p.X, p.Y] = new Cell(this, p);
    }
    private Player CreatePlayer()
    {
      var player = new Player(this);
      player.Hand = new HandCollection(PieceSet, player);
      return player;
    }
    private MovesHistory CreateMovesHistory()
    {
      var movesHistory = new MovesHistory();
      ((INotifyPropertyChanged)movesHistory).PropertyChanged += OnHistoryPropertyChanged;
      return movesHistory;
    }

    #endregion

    #region ' PieceSet + Set/Reset Piece '

    /// <summary>The set of pieces user has</summary>
    public IPieceSet PieceSet { get; private set; }

    /// <summary>Places the piece into the cell</summary>
    /// <remarks>Method takes ownerless piece and places it into the cell</remarks>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="piece"/> or <paramref name="owner"/> is null
    /// </exception>
    /// <exception cref="InvalidOperationException">The piece is not ownerless</exception>
    public void SetPiece(Piece piece, Position position, Player owner)
    {
      if (piece == null) throw new ArgumentNullException("piece");
      if (owner == null) throw new ArgumentNullException("owner");
      VerifyPlayerBelongs(owner);
      if (piece.Owner != null)
      {
        throw new InvalidOperationException(
          "Piece can't be in two places at the same time. " +
          "First return it to the piece set, then try to add it to the hand");
      }

      piece.Owner = owner;

      PieceSet.AcquirePiece(piece);
      GetCellAt(position).Piece = piece;
    }
    /// <summary>Set piece to the board cell</summary>
    public void SetPiece(IPieceType pieceType, Position position, Player owner)
    {
      if (pieceType == null) throw new ArgumentNullException("pieceType");

      var piece = PieceSet[pieceType];
      if (piece == null)
      {
        throw new NotEnoughPiecesInSetException(
          "Cannot set piece because there's no more pieces of type " +
          pieceType + " in the set. Consider using infinite piece set");
      }

      SetPiece(piece, position, owner);
    }
    /// <summary>Places the piece into the cell</summary>
    /// <remarks>Method takes <b>owned</b> piece and places it into the cell</remarks>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="piece"/> is null
    /// </exception>
    /// <exception cref="PieceHasNoOwnerException">the piece has no owner</exception>
    public void SetPiece(Piece piece, Position position)
    {
      if (piece == null) throw new ArgumentNullException("piece");
      if (GetCellAt(position).Piece == piece) return;

      var player = piece.Owner;
      if (player == null)
        throw new PieceHasNoOwnerException();
      PieceSet.ReleasePiece(piece);
      SetPiece(piece, position, player);
    }
    /// <summary>Removes the piece from the cell to the piece set</summary>
    public Piece ResetPiece(Position position)
    {
      if (GetCellAt(position).Piece == null) return null;
      var old = GetCellAt(position).Piece;
      GetCellAt(position).Piece = null;
      PieceSet.ReleasePiece(old);
      return old;
    }
    /// <summary>Returns all pieces from the board and
    /// both hands to <see cref="PieceSet"/> </summary>
    public void ResetAll()
    {
      foreach (var cell in Cells.Where(cell => cell.Piece != null))
        ResetPiece(cell.Position);

      White.Hand.Clear();
      Black.Hand.Clear();
    }

    #endregion

    #region ' Get/Make Move '

    /// <summary>Gets usual move on the board</summary>
    public UsualMove GetUsualMove(Position from, Position to, bool isPromoting)
    {
      if (!IsMovesOrderMaintained && GetPieceAt(from) != null)
        OneWhoMoves = GetPieceAt(from).Owner;

      return new UsualMove(OneWhoMoves.Color, from, to, isPromoting);
    }
    /// <summary>Gets drop move on the board</summary>
    public DropMove GetDropMove(IPieceType piece, Position to, Player who)
    {
      if (!IsMovesOrderMaintained)
        OneWhoMoves = who;

      return new DropMove(piece.GetColored(who.Color), to);
    }
    /// <summary>Gets drop move on the board</summary>
    public DropMove GetDropMove(Piece piece, Position to)
    {
      if (piece == null) throw new ArgumentNullException("piece");
      if (piece.Owner == null) throw new PieceHasNoOwnerException();
      if (piece.Owner.Board != this)
        throw new ArgumentOutOfRangeException("piece", "piece doesn't belong to the board");
      if (!piece.Owner.Hand.Contains(piece))
        throw new ArgumentOutOfRangeException("piece", "user has no that piece on the hand");

      if (!IsMovesOrderMaintained)
        OneWhoMoves = piece.Owner;

      return new DropMove(piece.Snapshot(), to);
    }
    /// <summary>Gets resign move</summary>
    public DecoratedMove GetResignMove()
    {
      return new DecoratedMove(CurrentSnapshot,
        new ResignMove(OneWhoMoves.Color),
        History.Count + 1);
    }

    /// <summary>Gets move on the board parsing it from snapsot</summary>
    public DecoratedMove Wrap(Move snapshot)
    {
      // TODO: Using of this method is almost always ugly!
      if (snapshot == null) throw new ArgumentNullException("snapshot");
      return new DecoratedMove(CurrentSnapshot, snapshot, History.Count + 1);
    }
    public void MakeWrapedMove(Move move)
    {
      // TODO: Rename it to MakeMove
      MakeMove(Wrap(move));
    }

    /// <summary>Makes the move on the board</summary>
    /// <remarks>The method adds the move to the history and sends events</remarks>
    public void MakeMove(DecoratedMove move)
    {
      if (move == null) throw new ArgumentNullException("move");
      if (!move.IsValid) throw new InvalidMoveException(move.RulesViolation);

      using (_moving.Set())
      {
        OnMoving(new MoveEventArgs(move));
        MakeMoveInternal(move.Move);
        _oneWhoMoves = _oneWhoMoves.Opponent;
        History.Do(move);
        OnMoved(new MoveEventArgs(move));
      }
    }

    #endregion

    #region ' Analysis '

    /// <summary>Gets all valid usual moves available from the given position</summary>
    public IEnumerable<UsualMove> GetAvailableMoves(Position fromPosition)
    {
      if (!IsMovesOrderMaintained)
      {
        var piece = GetPieceAt(fromPosition);
        if (piece != null) OneWhoMoves = piece.Owner;
      }
      return from snapshot in CurrentSnapshot.GetAvailableUsualMoves(fromPosition)
             select snapshot;
    }
    /// <summary>Gets all valid drop moves available for the player for the given piece type</summary>
    public IEnumerable<DropMove> GetAvailableMoves(IPieceType pieceType, PieceColor color)
    {
      if (pieceType == null) throw new ArgumentNullException("pieceType");

      var piece = GetPlayer(color).Hand.GetByType(pieceType);
      if (piece == null)
        throw new PieceNotFoundException(pieceType, string.Format(
          "The piece of type {0} is not found in {1} hand.", pieceType, color));

      return GetAvailableMoves(piece);
    }
    /// <summary>Gets all valid drop moves available for for the given piece in hand</summary>
    public IEnumerable<DropMove> GetAvailableMoves(Piece piece)
    {
      if (piece == null) throw new ArgumentNullException("piece");
      if (piece.Owner == null) throw new PieceHasNoOwnerException();

      return from snapshot in CurrentSnapshot.GetAvailableDropMoves(piece.PieceType, piece.Color)
             select snapshot;
    }

    #endregion

    #region ' Events '

    /// <summary>Raised before move is done</summary>
    public event EventHandler<MoveEventArgs> Moving;
    /// <summary>Raised after move is done</summary>
    public event EventHandler<MoveEventArgs> Moved;
    /// <summary>Raised before user navigates in history</summary>
    public event EventHandler<HistoryNavigateEventArgs> HistoryNavigating;
    /// <summary>Raised after user navigated in history</summary>
    public event EventHandler<HistoryNavigateEventArgs> HistoryNavigated;

    #endregion

    private void OnMoving(MoveEventArgs e)
    {
      var handler = Moving;
      if (handler != null) handler(this, e);
    }
    private void OnMoved(MoveEventArgs e)
    {
      var handler = Moved;
      if (handler != null) handler(this, e);
    }
    private void OnHistoryNavigating(int diff, BoardSnapshot snapshot)
    {
      var handler = HistoryNavigating;
      if (handler != null) handler(this,
        new HistoryNavigateEventArgs(diff, snapshot));
    }
    private void OnHistoryNavigated(int diff, BoardSnapshot snapshot)
    {
      var handler = HistoryNavigated;
      if (handler != null) handler(this,
        new HistoryNavigateEventArgs(diff, snapshot));
    }
    private void OnHistoryPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "CurrentMoveIndex")
      {
        OnCurrentMoveIndexChanged();
      }
    }
    private void OnCurrentMoveIndexChanged()
    {
      var diff = History.CurrentMoveIndex - _currentMoveIndex;
      _currentMoveIndex = History.CurrentMoveIndex;
      if (_moving) return;

      var snapshot = History.CurrentSnapshot;
      OnHistoryNavigating(diff, snapshot);
      LoadSnapshot(snapshot);
      OnHistoryNavigated(diff, snapshot);
    }
    private void ResetCurrentSnapshot()
    {
      if (_currentSnapshot == null) return;
      RaisePropertyChanged("CurrentSnapshot");
      _currentSnapshot = null;
    }

    internal void VerifyPlayerBelongs(Player player)
    {
      if (player != White && player != Black)
        throw new ArgumentOutOfRangeException("player", "Player doesn't belong to this board");
    }

    private BoardSnapshot TakeSnapshot()
    {
      return new BoardSnapshot(OneWhoMoves.Color,

          from position in Position.OnBoard
          let piece = GetPieceAt(position)
          where piece != null
          select Tuple.Create(position, piece.Snapshot()),

          from whitePiece in White.Hand
          orderby whitePiece.PieceType
          select whitePiece.PieceType,

          from blackPiece in Black.Hand
          orderby blackPiece.PieceType
          select blackPiece.PieceType
        );
    }

    /// <summary>Notification from HandCollection</summary>
    internal void HandCollectionChanged()
    {
      ResetCurrentSnapshot();
    }
    /// <summary>Notification from Cell</summary>
    internal void CellPieceChanged()
    {
      ResetCurrentSnapshot();
    }

    private void MakeMoveInternal(Move move)
    {
      // TODO: if (CurrentSnapshot != move.BoardSnapshot) throw new Exception();
      var dropMove = move as DropMove;
      if (dropMove != null)
      {
        MakeDropMove(dropMove);
      }
      var usualMove = move as UsualMove;
      if (usualMove != null)
      {
        MakeUsualMove(usualMove);
      }
      var resignMove = move as ResignMove;
      if (resignMove != null)
      {
        GameResult = resignMove.Who == PieceColor.White ? 
          ShogiGameResult.BlackWin : ShogiGameResult.WhiteWin;
      }
    }
    private void MakeDropMove(DropMove move)
    {
      var player = GetPlayer(move.Who);
      var piece = player.Hand.GetByType(move.Piece.PieceType);
      player.Hand.Remove(piece);
      SetPiece(piece, move.To, player);
    }
    private void MakeUsualMove(UsualMove move)
    {
      var player = GetPlayer(move.Who);
      var piece = GetPieceAt(move.From);
      ResetPiece(move.From);
      if (move.IsPromoting) piece.IsPromoted = true;
      var targetPiece = GetPieceAt(move.To);
      if (targetPiece != null)
      {
        ResetPiece(move.To);
        player.Hand.Add(targetPiece);
      }
      SetPiece(piece, move.To, player);
    }
  }
}