using System;

namespace Yasc.ShogiCore.Core
{
  /// <summary>This exception is thrown when you'tr trying 
  ///   to make invalid move (<see cref="Board.MakeMove"/>)</summary>
  [Serializable]
  public class InvalidMoveException : Exception
  {
    internal InvalidMoveException(string message)
      : base(message)
    {
    }
  }
}