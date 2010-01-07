using System;

namespace Yasc.ShogiCore.Moves
{
  /// <summary>Holds move related data</summary>
  public class MoveEventArgs : EventArgs
  {
    /// <summary>Move</summary>
    public MoveBase Move { get; private set; }

    /// <summary>ctor</summary>
    public MoveEventArgs(MoveBase move)
    {
      Move = move;
    }
  }
}