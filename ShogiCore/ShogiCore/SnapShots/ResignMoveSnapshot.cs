using System;
using Yasc.ShogiCore.Primitives;

namespace Yasc.ShogiCore.Snapshots
{
  /// <summary>Represents lightweight snapshot of resign move</summary>
  [Serializable]
  public sealed class ResignMoveSnapshot : MoveSnapshotBase
  {
    private readonly PieceColor _who;

    /// <summary>Gets the color of player who made the move</summary>
    public override PieceColor Who
    {
      get { return _who; }
    }

    /// <summary>ctor</summary>
    public ResignMoveSnapshot(PieceColor who)
    {
      _who = who;
    }
  }
}