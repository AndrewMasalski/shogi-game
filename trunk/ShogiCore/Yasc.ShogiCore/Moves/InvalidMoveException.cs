using System;

namespace Yasc.ShogiCore.Moves
{
  [Serializable]
  public class InvalidMoveException : Exception
  {
    public InvalidMoveException(string message)
      : base(message)
    {
    }
  }
}