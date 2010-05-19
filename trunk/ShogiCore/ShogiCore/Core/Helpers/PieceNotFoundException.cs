using System;
using System.Runtime.Serialization;
using Yasc.ShogiCore.Primitives;

namespace Yasc.ShogiCore.Core
{
  /// <summary>The exception is fired when piece is not found by type</summary>
  [Serializable]
  public sealed class PieceNotFoundException : Exception
  {
    private PieceNotFoundException(SerializationInfo info, StreamingContext context) 
      : base(info, context)
    {
      if (info == null) throw new ArgumentNullException("info");
      PieceType = (PieceType) info.GetString("PieceType");
    }
    /// <summary>Partisipates serialization</summary>
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      if (info == null) throw new ArgumentNullException("info");
      info.AddValue("PieceType", PieceType.ToString());
    }

    /// <summary>The type of piece which couldn't be found</summary>
    public PieceType PieceType { get; private set; }

    private const string DefaultMessage =
      "The piece of type {0} is not found.";

    internal PieceNotFoundException(PieceType pieceType)
      : base(string.Format(DefaultMessage, pieceType))
    {
      PieceType = pieceType;
    }

    internal PieceNotFoundException(PieceType pieceType, string message)
      : base(message)
    {
      PieceType = pieceType;
    }
  }
}