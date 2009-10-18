using System;

namespace Yasc.ShogiCore.Moves.Validation
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