using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MvvmFoundation.Wpf;
using Yasc.Networking;
using Yasc.ShogiCore.Moves;
using Yasc.ShogiCore.SnapShots;
using Yasc.Utils;

namespace Yasc.ShogiCore
{
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
    public Player White { get; private set; }
    public Player Black { get; private set; }
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
    public MovesHistory History { get; private set; }
    public IEnumerable<Cell> Cells
    {
      get
      {
        foreach (var p in Position.OnBoard)
          yield return _cells[p.X, p.Y];
      }
    }
    public Piece this[Position c]
    {
      get { return _cells[c.X, c.Y].Piece; }
    }
    public Cell this[int x, int y]
    {
      get { return _cells[x, y]; }
    }
    public Player this[PieceColor color]
    {
      get { return color == PieceColor.White ? White : Black; }
    }

    #endregion

    #region ' Ctors, Load '

    public Board()
      : this(PieceSetType.Default)
    {
    }
    public Board(PieceSetType pieceSetType)
    {
      switch (pieceSetType)
      {
        case PieceSetType.Inifinite:
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

    public void LoadSnapshot(BoardSnapshot snapshot)
    {
      OneWhoMoves = this[snapshot.OneWhoMoves];

      ResetAll();

      foreach (var p in Position.OnBoard)
        if (snapshot[p] != null)
        {
          var piece = PieceSet[snapshot[p].Type];
          if (piece == null)
          {
            throw new NotEnoughtPiecesInSetException(
              "Can't load snapshot because it's not enough pieces in set: "+
              "couldn't find " + snapshot[p].Type + " to fill " + p);
          }
          SetPiece(p, piece, this[snapshot[p].Color]);
        }

      White.LoadSnapshot(snapshot.WhiteHand);
      Black.LoadSnapshot(snapshot.BlackHand);
    }

    #endregion

    #region ' Set/Reset Piece '

    public void SetPiece(Position p, Piece piece, Player owner)
    {
      this[p.X, p.Y].SetPiece(piece, owner);
    }
    public void SetPiece(Position p, Piece piece, PieceColor color)
    {
      SetPiece(p, piece, this[color]);
    }
    public void SetPiece(Position p, Player owner, PieceType type)
    {
      var piece = PieceSet[type];
      if (piece == null)
      {
        throw new NotEnoughtPiecesInSetException(
          "Cannot set piece because there's no more pieces of type " +
          type + " in the set. Consider using Infinite PieceSet");
      }

      SetPiece(p, piece, owner);
    }
    public void SetPiece(Position p, PieceColor color, PieceType type)
    {
      SetPiece(p, this[color], type);
    }
    public void SetPiece(Position p, Piece piece)
    {
      if (piece.Owner == null) throw new PieceHasNoOwnerException();
      SetPiece(p, piece, piece.Owner);
    }
    public void ResetPiece(Position p)
    {
      this[p.X, p.Y].ResetPiece();
    }

    #endregion

    #region ' Get/Make Move '

    public UsualMove GetUsualMove(Position from, Position to, bool isPromoting)
    {
      if (!IsMovesOrderMaintained && this[from] != null)
        OneWhoMoves = this[from].Owner;

      return UsualMove.Create(this, from, to, isPromoting);
    }
    public DropMove GetDropMove(PieceType piece, Position to, Player who)
    {
      if (!IsMovesOrderMaintained)
        OneWhoMoves = who;

      return DropMove.Create(this, piece, to, who);
    }
    public DropMove GetDropMove(Piece piece, Position to)
    {
      if (piece == null) throw new ArgumentNullException("piece");
      if (piece.Owner == null) throw new PieceHasNoOwnerException();

      if (!IsMovesOrderMaintained)
        OneWhoMoves = piece.Owner;

      if (piece.Owner.Board != this)
        throw new ArgumentOutOfRangeException("piece");

      return DropMove.Create(this, piece.Type, to, piece.Owner);
    }
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
    public MoveBase GetMove(MoveMsg msg)
    {
      var m = GetMove(msg.Move);
      m.CorrectTimeStamp(msg.TimeStamp);
      return m;
    }

    public void MakeMove(MoveBase move)
    {
      if (move == null) throw new ArgumentNullException("move");
      if (move.Board != this) throw new ArgumentOutOfRangeException("move");
      if (!move.IsValid) throw new InvalidMoveException(move.ErrorMessage);

      using (_moving.Set())
      {
        OnMoving(new MoveEventArgs(move));
        move.Make();
        _oneWhoMoves = _oneWhoMoves.Oppenent;
        History.Do(move);
        OnMoved(new MoveEventArgs(move));
      }
    }

    #endregion

    #region ' Analysis '

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
    public IEnumerable<DropMove> GetAvailableMoves(PieceType piece, PieceColor color)
    {
      return GetAvailableMoves(this[color].GetPieceFromHandByType(piece));
    }
    public IEnumerable<DropMove> GetAvailableMoves(Piece piece)
    {
      if (piece == null) throw new ArgumentNullException("piece");
      if (piece.Owner == null) throw new PieceHasNoOwnerException();

      return from snapshot in CurrentSnapshot.GetAvailableDropMoves(new PieceSnapshot(piece))
             select snapshot.AsRealMove(this);
    }

    #endregion

    #region ' Events '

    public event EventHandler<MoveEventArgs> Moving;
    public event EventHandler<MoveEventArgs> Moved;
    public event EventHandler<HistoryNavigateEventArgs> HistoryNavigating;
    public event EventHandler<HistoryNavigateEventArgs> HistoryNavigated;

    #endregion

    #region ' PieceSet '

    /// <summary>Returns all pieces from the board and
    /// both hands to <see cref="PieceSet"/> </summary>
    public void ResetAll()
    {
      foreach (var cell in Cells)
        if (cell.Piece != null)
          PieceSet.Return(cell.ResetPiece());

      White.ResetAllPiecesFromHand();
      Black.ResetAllPiecesFromHand();
    }
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

    internal void EnsurePlayerBelongs(Player player)
    {
      if (player != White && player != Black)
        throw new ArgumentOutOfRangeException("player", "Player doesn't belong to this board");
    }
  }

  public class PieceHasNoOwnerException : Exception
  {
    public const string DefaultMessage =
      "You can't pass " +
      "to this method piece from the PieceSet. " +
      "Pass piece from hand instead";

    public PieceHasNoOwnerException()
      : base(DefaultMessage)
    {
    }

    public PieceHasNoOwnerException(string message)
      : base(message)
    {
    }
  }

  public class NotEnoughtPiecesInSetException : Exception
  {
    public NotEnoughtPiecesInSetException(string message) 
      : base(message)
    {
    }
  }
}