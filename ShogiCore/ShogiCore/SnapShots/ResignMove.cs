using System;
using Yasc.ShogiCore.Primitives;

namespace Yasc.ShogiCore.Snapshots
{
  /// <summary>Represents lightweight snapshot of resign move</summary>
  [Serializable]
  public sealed class ResignMove : Move
  {
    private readonly PieceColor _who;

    /// <summary>Gets the color of player who made the move</summary>
    public override PieceColor Who
    {
      get { return _who; }
    }

    /// <summary>ctor</summary>
    public ResignMove(PieceColor who)
    {
      _who = who;
    }

    /// <summary>Returns user friendly move representation (latin)</summary>
    public override string ToString()
    {
      return "Resign";
    }
  }
}