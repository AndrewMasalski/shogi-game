using System;

namespace Yasc.ShogiCore
{
  /// <summary>The exception is fired when piece has no ownt but must have</summary>
  public class PieceHasNoOwnerException : Exception
  {
    private const string DefaultMessage =
      "You can't pass " +
      "to this method piece from the PieceSet. " +
      "Pass piece from hand instead";

    /// <summary>ctor</summary>
    public PieceHasNoOwnerException()
      : base(DefaultMessage)
    {
    }

    /// <summary>ctor</summary>
    public PieceHasNoOwnerException(string message)
      : base(message)
    {
    }
  }
}