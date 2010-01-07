using System;

namespace Yasc.ShogiCore.Moves
{
  public class MoveEventArgs : EventArgs
  {
    public MoveBase Move { get; private set; }

    public MoveEventArgs(MoveBase move)
    {
      Move = move;
    }
  }
}