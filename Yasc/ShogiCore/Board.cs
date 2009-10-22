using System;
using System.Collections;
using System.Collections.Generic;
using Yasc.ShogiCore.Moves;
using Yasc.ShogiCore.Moves.Validation;
using Yasc.ShogiCore.Utils;

namespace Yasc.ShogiCore
{
  public class Board : ViewModelBase, IEnumerable<Cell>
  {
    private Player _oneWhoMoves;
    private bool _isMovesOrderMaintained = true;
    private readonly Cell[,] _cells = new Cell[9, 9];
    private BoardSnapshot _currentSnapshot;

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
        OnPropertyChanged("OneWhoMoves");
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
        OnPropertyChanged("IsMovesOrderMaintained");
      }
    }
    public MovesHistory History { get; private set; }

    public Board()
    {
      History = new MovesHistory();

      White = new Player(this);
      Black = new Player(this);
      _oneWhoMoves = White;

      foreach (var p in Position.OnBoard)
      {
        var cell = new Cell(p);
        cell.PropertyChanged += (s, e) => ResetCurrentSnapshot();
        _cells[p.X, p.Y] = cell;
      }
    }

    private void ResetCurrentSnapshot()
    {
      if (_currentSnapshot == null) return;
      OnPropertyChanged("CurrentSnapshot");
      _currentSnapshot = null;
    }

    public Piece this[Position c]
    {
      get { return _cells[c.X, c.Y].Piece; }
      set { _cells[c.X, c.Y].Piece = value; }
    }
    public Cell this[int x, int y]
    {
      get { return _cells[x, y]; }
    }
    public Player this[PieceColor color]
    {
      get { return color == PieceColor.White ? White : Black; }
    }
    public UsualMove GetUsualMove(Position from, Position to, bool isPromoting)
    {
      if (!IsMovesOrderMaintained && this[from] != null) 
        OneWhoMoves = this[from].Owner;

      return UsualMove.Create(this, from, to, isPromoting);
    }
    public DropMove GetDropMove(Piece piece, Position to)
    {
      if (piece == null) throw new ArgumentNullException("piece");

      if (!IsMovesOrderMaintained)
        OneWhoMoves = piece.Owner;

      if (piece.Owner.Board != this)
        throw new ArgumentOutOfRangeException("piece");

      return DropMove.Create(this, piece, to);
    }
    public MoveBase GetMove(string text)
    {
      if (text.Contains("-"))
      {
        var elements = text.Split('-');
        var from = (Position) elements[0];
        var to = (Position) elements[1].Substring(0, 2);
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

        return DropMove.Create(this,
          OneWhoMoves.GetPieceFromHandByType(piece), to);
      }
    }

    public void MakeMove(MoveBase move)
    {
      if (move == null) throw new ArgumentNullException("move");
      if (move.Board != this) throw new ArgumentOutOfRangeException("move");
      if (!move.IsValid) throw new InvalidMoveException(move.ErrorMessage);

      move.Make();
      _oneWhoMoves = _oneWhoMoves.Oppenent;
      History.Do(move);
    }

    #region ' Implementation of IEnumerable '

    IEnumerator<Cell> IEnumerable<Cell>.GetEnumerator()
    {
      foreach (var p in Position.OnBoard)
        yield return _cells[p.X, p.Y];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable<Cell>)this).GetEnumerator();
    }

    #endregion
  }
}