using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Yasc.ShogiCore.Moves;
using Yasc.ShogiCore.Utils;
using Yasc.Utils;

namespace Yasc.ShogiCore
{
  [Serializable]
  public class BoardSnapshot
  {
    #region ' Fields '

    private readonly int _hashCode;
    private readonly PieceSnapshot[,] _cells = new PieceSnapshot[9, 9];
    private readonly List<PieceSnapshot> _blackHand;
    private readonly List<PieceSnapshot> _whiteHand;

    private ReadOnlySquareArray<PieceSnapshot> _cellsRo;
    private ReadOnlyCollection<PieceSnapshot> _blackHandRo;
    private ReadOnlyCollection<PieceSnapshot> _whiteHandRo;

    #endregion

    #region ' Public Interface '

    public PieceColor OneWhoMoves { get; private set; }
    public ReadOnlySquareArray<PieceSnapshot> Cells
    {
      get
      {
        if (_cellsRo == null)
        {
          _cellsRo = new ReadOnlySquareArray<PieceSnapshot>(_cells);
        }
        return _cellsRo;
      }
    }
    public ReadOnlyCollection<PieceSnapshot> BlackHand
    {
      get
      {
        if (_blackHandRo == null)
        {
          _blackHandRo = new ReadOnlyCollection<PieceSnapshot>(_blackHand);
        }
        return _blackHandRo;
      }
    }
    public ReadOnlyCollection<PieceSnapshot> WhiteHand
    {
      get
      {
        if (_whiteHandRo == null)
        {
          _whiteHandRo = new ReadOnlyCollection<PieceSnapshot>(_whiteHand);
        }
        return _whiteHandRo;
      }
    }

    public bool IsMateFor(PieceColor color)
    {
      return SituationAnalizer.IsMateFor(this, color);
    }
    public bool IsCheckFor(PieceColor color)
    {
      return SituationAnalizer.IsCheckFor(this, color);
    }

    public BoardSnapshot(Board board)
      : this(board, null)
    {
    }
    public BoardSnapshot(Board board, MoveSnapshotBase move)
    {
      OneWhoMoves = board.OneWhoMoves.Color;

      foreach (var p in Position.OnBoard)
        this[p] = board[p] == null ? null : new PieceSnapshot(board[p]);

      _whiteHand = (from p in board.White.Hand select new PieceSnapshot(p)).ToList();
      _blackHand = (from p in board.Black.Hand select new PieceSnapshot(p)).ToList();

      if (move != null)
      {
        if (move is UsualMoveSnapshot)
        {
          Move((UsualMoveSnapshot)move);
        }
        else
        {
          Move((DropMoveSnapshot)move);
        }
      }

      _hashCode = CalculateHashcode();
    }

    public BoardSnapshot(BoardSnapshot board, MoveSnapshotBase move)
    {
      OneWhoMoves = board.OneWhoMoves;

      foreach (var p in Position.OnBoard)
        this[p] = board[p];

      _whiteHand = board.WhiteHand.ToList();
      _blackHand = board.BlackHand.ToList();

      if (move != null)
      {
        if (move is UsualMoveSnapshot)
        {
          Move((UsualMoveSnapshot)move);
        }
        else
        {
          Move((DropMoveSnapshot)move);
        }
      }
      _hashCode = CalculateHashcode();

    }

    public PieceSnapshot this[Position c]
    {
      get { return _cells[c.X, c.Y]; }
      private set { _cells[c.X, c.Y] = value; }
    }
    public PieceSnapshot this[int x, int y]
    {
      get { return _cells[x, y]; }
    }
    public ReadOnlyCollection<PieceSnapshot> Hand(PieceColor color)
    {
      return color == PieceColor.White ? WhiteHand : BlackHand;
    }

    #endregion

    #region ' Implemetation '

    private void Move(DropMoveSnapshot move)
    {
      HandInternal(OneWhoMoves).Remove(move.Piece);
      this[move.To] = move.Piece;
    }
    private void Move(UsualMoveSnapshot move)
    {
      if (this[move.From] == null) return;

      if (move.IsPromoting)
        this[move.From] = this[move.From].ClonePromoted();

      if (move.From == move.To) return;

      if (this[move.To] != null)
        HandInternal(OneWhoMoves).Add(this[move.To]);
      this[move.To] = this[move.From];
      this[move.From] = null;
    }
    private List<PieceSnapshot> HandInternal(PieceColor color)
    {
      return color == PieceColor.White ? _whiteHand : _blackHand;
    }
    private int CalculateHashcode()
    {
      return ListUtils.GetSeqHashcode(
        OneWhoMoves.GetHashCode(),
        _cells.GetSeqHashcode(),
        _whiteHand.GetHashcode(),
        _blackHand.GetHashcode());
    }

    #endregion

    #region ' Equals '

    public bool Equals(BoardSnapshot other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return other._hashCode == _hashCode &&
             Equals(other.OneWhoMoves, OneWhoMoves) &&
             ListUtils.Equal(other.Cells, Cells) &&
             ListUtils.Equivalent(other.BlackHand, BlackHand) &&
             ListUtils.Equivalent(other.WhiteHand, WhiteHand);
    }
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != typeof(BoardSnapshot)) return false;
      return Equals((BoardSnapshot)obj);
    }
    public override int GetHashCode()
    {
      return _hashCode;
    }
    public static bool operator ==(BoardSnapshot left, BoardSnapshot right)
    {
      return Equals(left, right);
    }
    public static bool operator !=(BoardSnapshot left, BoardSnapshot right)
    {
      return !Equals(left, right);
    }

    #endregion
  }
}