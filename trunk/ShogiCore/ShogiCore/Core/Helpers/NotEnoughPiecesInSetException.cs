using System;
using System.Runtime.Serialization;

namespace Yasc.ShogiCore.Core
{
  /// <summary>Exception if fired when user is trying to use  
  ///   piece when all pieces from set have been already used</summary>
  [Serializable]
  public sealed class NotEnoughPiecesInSetException : Exception
  {
    internal NotEnoughPiecesInSetException(string message)
      : base(message)
    {
    }

    internal NotEnoughPiecesInSetException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    private NotEnoughPiecesInSetException(SerializationInfo info, StreamingContext context) 
      : base(info, context)
    {
    }
  }
}