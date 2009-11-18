using System;
using Yasc.ShogiCore.Utils;

namespace Yasc.ShogiCore.Moves
{
  [Serializable]
  public class UsualMoveSnapshot : MoveSnapshotBase
  {
    public Position From { get; private set; }
    public Position To { get; private set; }
    public bool IsPromoting { get; private set; }

    public UsualMoveSnapshot(UsualMove move)
    {
      From = move.From;
      To = move.To;
      IsPromoting = move.IsPromoting;
    }

    public UsualMoveSnapshot(Position from, Position to, bool isPromoting)
    {
      From = from;
      To = to;
      IsPromoting = isPromoting;
    }

    public override MoveBase AsRealMove(Board board)
    {
      return board.GetUsualMove(From, To, IsPromoting);
    }

    public override PieceColor GetColor(BoardSnapshot snapshot)
    {
      return snapshot[From].Color;
    }

    public override string ToString()
    {
      return From + "-" + To + (IsPromoting ? "+" : "");
    }
  }
}