using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Yasc.ShogiCore.PieceSets;
using Yasc.ShogiCore.Primitives;
using Yasc.ShogiCore.Snapshots;
using Yasc.Utils;
using Yasc.Utils.Mvvm;

namespace Yasc.ShogiCore.Core
{
  /// <summary>Represents observable and mutable shogi board with moves tracking, analysis, etc.</summary>
  public class Board : ObservableObject
  {
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
    /// <summary>Gets the piece in the cell in the position -or- null if the cell is empty</summary>
    public Piece this[Position position]
    {
      get { return _cells[position.X, position.Y].Piece; }
    }
    /// <summary>Gets the cell in the position</summary>
    public Cell this[int x, int y]
    {
      get { return _cells[x, y]; }
    }
    /// <summary>Gets the player of the given color</summary>
    public Player this[PieceColor color]
    {
      get { return color == PieceColor.White ? White : Black; }
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
    public Board()
      : this(new StandardPieceSet())
    {
    }
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
    public void LoadSnapshot(BoardSnapshot snapshot)
    {
      if (snapshot == null) throw new ArgumentNullException("snapshot");
      OneWhoMoves = this[snapshot.OneWhoMoves];

      ResetAll();

      foreach (var p in Position.OnBoard)
        if (snapshot[p] != null)
        {
          var piece = PieceSet[snapshot[p].PieceType];
          if (piece == null)
          {
            throw new NotEnoughPiecesInSetException(
              "Can't load snapshot because it's not enough pieces in set: " +
              "couldn't find " + snapshot[p].PieceType + " to fill " + p);
          }
          SetPiece(piece, snapshot[p].Color, p);
        }

      White.Hand.LoadSnapshot(snapshot.WhiteHand);
      Black.Hand.LoadSnapshot(snapshot.BlackHand);
    }

    private void FillCells()
    {
      foreach (var p in Position.OnBoard)
      {
        var cell = new Cell(p);
        cell.PropertyChanged += (s, e) => ResetCurrentSnapshot();
        _cells[p.X, p.Y] = cell;
      }
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

    #region ' Set/Reset Piece '

    /// <summary>Set piece to the board cell</summary>
    public void SetPiece(Piece piece, Player forOwner, Position toPosition)
    {
      VerifyPlayerBelongs(forOwner);
      SetPiece(toPosition, piece, forOwner);
    }
    /// <summary>Set piece to the board cell</summary>
    public void SetPiece(Piece piece, PieceColor forOwner, Position toPosition)
    {
      SetPiece(piece, this[forOwner], toPosition);
    }
    /// <summary>Set piece to the board cell</summary>
    public void SetPiece(PieceType ofType, Player forOwner, Position toPosition)
    {
      VerifyPlayerBelongs(forOwner);
      var piece = PieceSet[ofType];
      if (piece == null)
      {
        throw new NotEnoughPiecesInSetException(
          "Cannot set piece because there's no more pieces of type " +
          ofType + " in the set. Consider using infinite piece set");
      }

      SetPiece(piece, forOwner, toPosition);
    }
    /// <summary>Set piece to the board cell</summary>
    public void SetPiece(PieceType ofType, PieceColor forOwner, Position toPosition)
    {
      SetPiece(ofType, this[forOwner], toPosition);
    }

    #endregion

    #region ' Get/Make Move '

    /// <summary>Gets usual move on the board</summary>
    public UsualMove GetUsualMove(Position from, Position to, bool isPromoting)
    {
      if (!IsMovesOrderMaintained && this[from] != null)
        OneWhoMoves = this[from].Owner;

      return UsualMove.Create(this, from, to, isPromoting);
    }
    /// <summary>Gets drop move on the board</summary>
    public DropMove GetDropMove(PieceType piece, Position to, Player who)
    {
      if (!IsMovesOrderMaintained)
        OneWhoMoves = who;

      return DropMove.Create(this, piece, to, who);
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

      return DropMove.Create(this, piece.PieceType, to, piece.Owner);
    }
    /// <summary>Gets resign move</summary>
    public ResignMove GetResignMove()
    {
      return new ResignMove(this, OneWhoMoves);
    }

    /// <summary>Gets move on the board parsing it from trascript</summary>
    public IEnumerable<MoveBase> GetMove(string text, INotation notation)
    {
      if (notation == null) throw new ArgumentNullException("notation");
      return notation.Parse(CurrentSnapshot, text).Select(GetMove);
    }

    /// <summary>Gets move on the board parsing it from snapsot</summary>
    public MoveBase GetMove(MoveSnapshotBase snapshot)
    {
      var usualMove = snapshot as UsualMoveSnapshot;
      if (usualMove != null) return GetMove(usualMove);
      var dropMove = snapshot as DropMoveSnapshot;
      if (dropMove != null) return GetMove(dropMove);
      var resignMove = snapshot as ResignMoveSnapshot;
      if (resignMove != null) return GetResignMove();
      throw new ArgumentOutOfRangeException("snapshot");
    }
    /// <summary>Gets move on the board parsing it from snapsot</summary>
    public UsualMove GetMove(UsualMoveSnapshot snapshot)
    {
      if (snapshot == null) throw new ArgumentNullException("snapshot");
      return GetUsualMove(snapshot.From, snapshot.To, snapshot.IsPromoting);
    }
    /// <summary>Gets move on the board parsing it from snapsot</summary>
    public DropMove GetMove(DropMoveSnapshot snapshot)
    {
      if (snapshot == null) throw new ArgumentNullException("snapshot");
      return GetDropMove(snapshot.Piece.PieceType, snapshot.To, this[snapshot.Who]);
    }
    /// <summary>Makes the move on the board</summary>
    /// <remarks>The method adds the move to the history and sends events</remarks>
    public void MakeMove(MoveBase move)
    {
      if (move == null) throw new ArgumentNullException("move");
      if (move.Board != this) throw new ArgumentOutOfRangeException("move");
      if (!move.IsValid) throw new InvalidMoveException(move.ErrorMessage);

      using (_moving.Set())
      {
        OnMoving(new MoveEventArgs(move));
        move.Make();
        _oneWhoMoves = _oneWhoMoves.Opponent;
        History.Do(move);
        OnMoved(new MoveEventArgs(move));
      }
    }

    #endregion

    #region ' Analysis '

    /// <summary>Gets all valid ususal moves available from the given position</summary>
    public IEnumerable<UsualMove> GetAvailableMoves(Position fromPosition)
    {
      if (!IsMovesOrderMaintained)
      {
        var piece = this[fromPosition];
        if (piece != null) OneWhoMoves = piece.Owner;
      }
      return from snapshot in CurrentSnapshot.GetAvailableUsualMoves(fromPosition)
             select GetMove(snapshot);
    }
    /// <summary>Gets all valid drop moves available for the player for the given piece type</summary>
    public IEnumerable<DropMove> GetAvailableMoves(PieceType pieceType, PieceColor color)
    {
      var piece = this[color].Hand.GetByType(pieceType);
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

      return from snapshot in CurrentSnapshot.GetAvailableDropMoves(piece.Snapshot())
             select GetMove(snapshot);
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

    #region ' PieceSet '

    /// <summary>Returns all pieces from the board and
    /// both hands to <see cref="PieceSet"/> </summary>
    public void ResetAll()
    {
      foreach (var cell in Cells.Where(cell => cell.Piece != null))
        ResetPiece(cell.Position);

      White.Hand.Clear();
      Black.Hand.Clear();
    }
    /// <summary>The set of pieces user has</summary>
    public IPieceSet PieceSet { get; private set; }

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

    #region ' Set\Reset piece in the cell '

    /// <summary>Places the piece into the cell</summary>
    /// <remarks>Method takes ownerless piece and places it into the cell</remarks>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="piece"/> or <paramref name="owner"/> is null
    /// </exception>
    /// <exception cref="InvalidOperationException">The piece is not ownerless</exception>
    public void SetPiece(Position position, Piece piece, Player owner)
    {
      if (piece == null) throw new ArgumentNullException("piece");
      if (owner == null) throw new ArgumentNullException("owner");
      if (piece.Owner != null)
      {
        throw new InvalidOperationException(
          "Piece can't be in two places at the same time. " +
          "First return it to the piece set, then try to add it to the hand");
      }

      piece.Owner = owner;

      PieceSet.AcquirePiece(piece);
      this[position.X, position.Y].Piece = piece;
    }
    /// <summary>Places the piece into the cell</summary>
    /// <remarks>Method takes piece and places it into the cell</remarks>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="piece"/> is null
    /// </exception>
    /// <exception cref="PieceHasNoOwnerException">the piece has no owner</exception>
    public void SetPiece(Position position, Piece piece)
    {
      if (piece == null) throw new ArgumentNullException("piece");
      if (this[position.X, position.Y].Piece == piece) return;

      var player = piece.Owner;
      if (player == null)
        throw new PieceHasNoOwnerException();
      PieceSet.ReleasePiece(piece);
      SetPiece(position, piece, player);
    }
    /// <summary>Removes the piece from the cell to the piece set</summary>
    public Piece ResetPiece(Position position)
    {
      if (this[position.X, position.Y].Piece == null) return null;
      var old = this[position.X, position.Y].Piece;
      this[position.X, position.Y].Piece = null;
      PieceSet.ReleasePiece(old);
      return old;
    }

    #endregion

    private BoardSnapshot TakeSnapshot()
    {
      return new BoardSnapshot(OneWhoMoves.Color,

          from position in Position.OnBoard
          let piece = this[position]
          where piece != null
          select Tuple.Create(position, piece.Snapshot()),

          from whitePiece in White.Hand
          orderby whitePiece.PieceType
          select whitePiece.Snapshot(),

          from blackPiece in Black.Hand
          orderby blackPiece.PieceType
          select blackPiece.Snapshot()
        );
    }
  }

  /// <summary>Describes one of the ways moves can be transcribed</summary>
  public interface INotation
  {
    /// <summary>Gets move on the board parsing it from transcript</summary>
    /// <param name="originalBoardState">State of the board before move</param>
    /// <param name="move">Move trancsript to parse</param>
    /// <returns>All moves which may be transcribed given way. 
    ///   Doesn't return null but be prepared to receive 0 moves.</returns>
    IEnumerable<MoveSnapshotBase> Parse(BoardSnapshot originalBoardState, string move);

    /// <summary>Returns the transcript for a given move</summary>
    /// <param name="originalBoardState">State of the board before move</param>
    /// <param name="move">Move to trancsript</param>
    string ToString(BoardSnapshot originalBoardState, MoveSnapshotBase move);
  }
}