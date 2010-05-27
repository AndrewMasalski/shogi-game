using System;

namespace Yasc.ShogiCore.Core
{
  /// <summary>Holds move related data</summary>
  public class MoveEventArgs : EventArgs
  {
    /// <summary>Move</summary>
    public DecoratedMove DecoratedMove { get; private set; }

    /// <summary>ctor</summary>
    public MoveEventArgs(DecoratedMove move)
    {
      DecoratedMove = move;
    }
  }
}