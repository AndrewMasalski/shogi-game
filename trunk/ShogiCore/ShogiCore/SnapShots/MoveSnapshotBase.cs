using System;

namespace Yasc.ShogiCore.Snapshots
{
  /// <summary>Base class for lightweight snapshots of usual and drop moves</summary>
  [Serializable]
  public abstract class MoveSnapshotBase
  {
    /// <summary>Gets the color of player who made the move</summary>
    public abstract PieceColor Who { get; }
  }
}