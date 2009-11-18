using System;
using Yasc.ShogiCore.Utils;

namespace Yasc.ShogiCore.Moves
{
  [Serializable]
  public abstract class MoveSnapshotBase
  {
    public abstract MoveBase AsRealMove(Board board);
    public abstract PieceColor GetColor(BoardSnapshot snapshot);
  }
}