using System;

namespace Yasc.ShogiCore.Moves
{
  [Serializable]
  public abstract class MoveSnapshotBase
  {
    public abstract MoveBase AsRealMove(Board board);
  }
}