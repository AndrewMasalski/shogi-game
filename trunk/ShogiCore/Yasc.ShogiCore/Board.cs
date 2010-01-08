using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MvvmFoundation.Wpf;
using Yasc.ShogiCore.Moves;
using Yasc.ShogiCore.Snapshots;
using Yasc.Utils;

namespace Yasc.ShogiCore
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

    #endregion

    #region ' Properties and Indexers '
    
    /// <summary>Snapshot of the board's current state</summary>
    public BoardSnapshot CurrentSnapshot
    {
      get
      {
        if (_currentSnapshot == null)
        {
          _currentSnapshot = new BoardSnapshot(this);
        }
        return _currentSnapshot;
      }
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
      get
      {
        foreach (var p in Position.OnBoard)
          yield return _cells[p.X, p.Y];
      }
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

    #endregion

    #region ' Ctors, Load '
    
    /// <summary>ctor</summary>
    public Board()
      : this(PieceSetType.Default)
    {
    }
    /// <summary>ctor</summary>
    public Board(PieceSetType pieceSetType)
    {
      switch (pieceSetType)
      {
        case PieceSetType.Infinite:
          PieceSet = new InfinitePieceSet();
          break;
        default:
          PieceSet = new DefaultPieceSet();
          break;
      }

      History = new MovesHistory();
      ((INotifyPropertyChanged)History).PropertyChanged += OnHistoryPropertyChanged;

      White = new Player(this);
      Black = new Player(this);
      _oneWhoMoves = White;

      foreach (var p in Position.OnBoard)
      {
        var cell = new Cell(this, p);
        cell.PropertyChanged += (s, e) => ResetCurrentSnapshot();
        _cells[p.X, p.Y] = cell;
      }
    }

    /// <summary>Loads the snapshot</summary>
    public void LoadSnapshot(BoardSnapshot snapshot)
    {
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

      White.LoadHandSnapshot(snapshot.WhiteHand);
      Black.LoadHandSnapshot(snapshot.BlackHand);
    }

    #endregion

    #region ' Set/Reset Piece '

    /// <summary>Set piece to the board cell</summary>
    public void SetPiece(Piece piece, Player forOwner, Position toPosition)
    {
      VerifyPlayerBelongs(forOwner);
      this[toPosition.X, toPosition.Y].SetPiece(piece, forOwner);
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
          ofType + " in the set. Consider using Infinite PieceSet");
      }

      SetPiece(piece, forOwner, toPosition);
    }
    /// <summary>Set piece to the board cell</summary>
    public void SetPiece(PieceType ofType, PieceColor forOwner, Position toPosition)
    {
      SetPiece(ofType, this[forOwner], toPosition);
    }
    /// <summary>Remove piece from the board cell</summary>
    public void ResetPiece(Position atPosition)
    {
      this[atPosition.X, atPosition.Y].ResetPiece();
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
        throw new ArgumentOutOfRangeException("piece");

      if (!IsMovesOrderMaintained)
        OneWhoMoves = piece.Owner;

      return DropMove.Create(this, piece.PieceType, to, piece.Owner);
    }
    
    /// <summary>Gets move on the board parsing it from transcript</summary>
    public IEnumerable<MoveBase> GetMove(string text, MoveNotation notation)
    {
      switch (notation)
      {
        case MoveNotation.Formal:
          yield return GetMove(text);
          break;
        case MoveNotation.Cute:
          foreach (var move in new CuteMoveParser(this).Parse(text))
            yield return move;
          break;
      }
    }
    /// <summary>Gets move on the board parsing it from transcript</summary>
    public MoveBase GetMove(string text)
    {
      if (text.Contains("-"))
      {
        var elements = text.Split('-');
        var from = (Position)elements[0];
        var to = (Position)elements[1].Substring(0, 2);
        var modifier = elements[1].Length > 2 ? elements[1].Substring(2, 1) : null;

        if (!IsMovesOrderMaintained && this[from] != null)
          OneWhoMoves = this[from].Owner;

        return UsualMove.Create(this, from, to, modifier == "+");
      }
      else
      {
        var elements = text.Split('\'');
        var piece = (PieceType)elements[0];
        var to = (Position)elements[1];

        return DropMove.Create(this, piece, to, OneWhoMoves);
      }
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
             select snapshot.AsRealMove(this);
    }
    /// <summary>Gets all valid drop moves available for the player for the given piece type</summary>
    public IEnumerable<DropMove> GetAvailableMoves(PieceType pieceType, PieceColor color)
    {
      var piece = this[color].GetPieceFromHandByType(pieceType);
#warning need to check for null
      return GetAvailableMoves(piece);
    }
    /// <summary>Gets all valid drop moves available for for the given piece in hand</summary>
    public IEnumerable<DropMove> GetAvailableMoves(Piece piece)
    {
      if (piece == null) throw new ArgumentNullException("piece");
      if (piece.Owner == null) throw new PieceHasNoOwnerException();

      return from snapshot in CurrentSnapshot.GetAvailableDropMoves(new PieceSnapshot(piece))
             select snapshot.AsRealMove(this);
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
      foreach (var cell in Cells)
        if (cell.Piece != null)
          cell.ResetPiece();

      White.ResetAllPiecesFromHand();
      Black.ResetAllPiecesFromHand();
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

      var snapshot = History.GetCurrentSnapshot();
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
  }
}