using System;
using System.Runtime.Serialization;

namespace Yasc.ShogiCore.Core
{
  /// <summary>This exception is thrown when you'tr trying 
  ///   to make invalid move (<see cref="Board.MakeMove"/>)</summary>
  [Serializable]
  public sealed class InvalidMoveException : Exception
  {
    internal InvalidMoveException(string message)
      : base(message)
    {
    }

    internal InvalidMoveException(string message, Exception innerException) 
      : base(message, innerException)
    {
    }

    private InvalidMoveException(SerializationInfo info, StreamingContext context) 
      : base(info, context)
    {
    }
  }
}