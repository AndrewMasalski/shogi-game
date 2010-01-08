using System;
using Yasc.ShogiCore.Moves;

namespace Yasc.ShogiCore.Snapshots
{
  /// <summary>Lightweight representation of the usual move (as opposing to <see cref="DropMoveSnapshot"/></summary>
  [Serializable]
  public sealed class UsualMoveSnapshot : MoveSnapshotBase
  {
    /// <summary>Position on the board move originates from</summary>
    public Position From { get; private set; }
    /// <summary>Position on the board piece is moving to</summary>
    public Position To { get; private set; }
    /// <summary>Indicates whether the move is promoting</summary>
    public bool IsPromoting { get; private set; }

    /// <summary>ctor</summary>
    public UsualMoveSnapshot(UsualMove move)
    {
      From = move.From;
      To = move.To;
      IsPromoting = move.IsPromoting;
    }
    /// <summary>ctor</summary>
    public UsualMoveSnapshot(Position from, Position to, bool isPromoting)
    {
      From = from;
      To = to;
      IsPromoting = isPromoting;
    }
    /// <summary>Creates observable move on the base of the snapshot</summary>
    public new UsualMove AsRealMove(Board board)
    {
      return board.GetUsualMove(From, To, IsPromoting);
    }
    public override PieceColor GetColor(BoardSnapshot snapshot)
    {
      return snapshot[From].Color;
    }
    protected override MoveBase AsRealMoveCore(Board board)
    {
      return AsRealMove(board);
    }

    public override string ToString()
    {
      return From + "-" + To + (IsPromoting ? "+" : "");
    }
  }
}