using System;
using Yasc.ShogiCore.Moves;
using Yasc.ShogiCore.Utils;

namespace Yasc.ShogiCore.SnapShots
{
  [Serializable]
  public abstract class MoveSnapshotBase
  {
    public abstract MoveBase AsRealMove(Board board);
    public abstract PieceColor GetColor(BoardSnapshot snapshot);
  }
}