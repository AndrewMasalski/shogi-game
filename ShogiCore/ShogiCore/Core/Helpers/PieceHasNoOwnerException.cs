using System;
using System.Runtime.Serialization;

namespace Yasc.ShogiCore.Core
{
  /// <summary>The exception is fired when piece has no owner but must have</summary>
  [Serializable]
  public sealed class PieceHasNoOwnerException : Exception
  {
    private const string DefaultMessage =
      "You can't pass " +
      "to this method piece from the piece set. " +
      "Pass piece from hand instead";

    /// <summary>ctor</summary>
    internal PieceHasNoOwnerException()
      : base(DefaultMessage)
    {
    }

    /// <summary>ctor</summary>
    internal PieceHasNoOwnerException(string message)
      : base(message)
    {
    }

    internal PieceHasNoOwnerException(string message, Exception innerException) 
      : base(message, innerException)
    {
    }

    private PieceHasNoOwnerException(SerializationInfo info, StreamingContext context) 
      : base(info, context)
    {
    }
  }
}