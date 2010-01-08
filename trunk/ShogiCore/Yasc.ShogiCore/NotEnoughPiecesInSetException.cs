using System;

namespace Yasc.ShogiCore
{
  /// <summary>Exception if fired when user is trying to use  
  ///   piece when all pieces from set have been already used</summary>
  public class NotEnoughPiecesInSetException : Exception
  {
    /// <summary>ctor</summary>
    public NotEnoughPiecesInSetException(string message)
      : base(message)
    {
    }
  }
}