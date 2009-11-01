using System;
using Yasc.ShogiCore.Moves;

namespace Yasc.ShogiCore
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