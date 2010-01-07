using System;
using Yasc.ShogiCore.Moves;

namespace Yasc.ShogiCore.Snapshots
{
  [Serializable]
  public abstract class MoveSnapshotBase
  {
    public MoveBase AsRealMove(Board board)
    {
      return AsRealMoveCore(board);
    }
    
    public abstract PieceColor GetColor(BoardSnapshot snapshot);
    protected abstract MoveBase AsRealMoveCore(Board board);
  }
}