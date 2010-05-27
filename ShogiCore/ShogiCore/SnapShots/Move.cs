using System;
using Yasc.ShogiCore.Primitives;

namespace Yasc.ShogiCore.Snapshots
{
  /// <summary>Base class for lightweight snapshots of usual and drop moves</summary>
  [Serializable]
  public abstract class Move
  {
    /// <summary>Gets the color of player who made the move</summary>
    public abstract PieceColor Who { get; }
  }
}