using System;
using Yasc.ShogiCore.Snapshots;

namespace Yasc.ShogiCore.Core
{
  /// <summary>Holds move related data</summary>
  public class MoveEventArgs : EventArgs
  {
    /// <summary>Move</summary>
    public Move Move { get; private set; }

    /// <summary>ctor</summary>
    public MoveEventArgs(Move move)
    {
      Move = move;
    }
  }
}