using System;
using Yasc.ShogiCore.Moves;

namespace Yasc.ShogiCore.Snapshots
{
  /// <summary>Base class for lightweight snapshots of usual and drop moves</summary>
  [Serializable]
  public abstract class MoveSnapshotBase
  {
    /// <summary>Gets observable version of the move</summary>
    public MoveBase AsRealMove(Board board)
    {
      return AsRealMoveCore(board);
    }
    
    /// <summary>Override to determine color of the player the move belongs to</summary>
    public abstract PieceColor GetColor(BoardSnapshot snapshot);
    /// <summary>Override to convert snapshot to observable move</summary>
    protected abstract MoveBase AsRealMoveCore(Board board);
  }
}