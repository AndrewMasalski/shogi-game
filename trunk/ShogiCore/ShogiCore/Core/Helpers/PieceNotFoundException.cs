using System;
using Yasc.ShogiCore.Primitives;

namespace Yasc.ShogiCore.Core
{
  /// <summary>The exception is fired when piece is not found by type</summary>
  public class PieceNotFoundException : Exception
  {
    /// <summary>The type of piece which couldn't be found</summary>
    public PieceType PieceType { get; private set; }

    private const string DefaultMessage =
      "The piece of type {0} is not found.";

    /// <summary>ctor</summary>
    public PieceNotFoundException(PieceType pieceType)
      : base(string.Format(DefaultMessage, pieceType))
    {
      PieceType = pieceType;
    }

    /// <summary>ctor</summary>
    public PieceNotFoundException(PieceType pieceType, string message)
      : base(message)
    {
      PieceType = pieceType;
    }
  }
}