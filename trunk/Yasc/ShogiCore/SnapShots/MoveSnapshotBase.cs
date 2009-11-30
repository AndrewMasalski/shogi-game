using System;
using Yasc.ShogiCore.Moves;

namespace Yasc.ShogiCore.SnapShots
{
  [Serializable]
  public abstract class MoveSnapshotBase
  {
    protected abstract MoveBase AsRealMoveCore(Board board);
    public MoveBase AsRealMove(Board board)
    {
      return AsRealMoveCore(board);
    }
    public abstract PieceColor GetColor(BoardSnapshot snapshot);
  }
}