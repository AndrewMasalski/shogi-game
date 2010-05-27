using System;
using Yasc.ShogiCore.Primitives;

namespace Yasc.ShogiCore.Snapshots
{
  /// <summary>Lightweight representation of drop move (as opposing to <see cref="UsualMove"/></summary>
  [Serializable]
  public sealed class DropMove : Move
  {
    /// <summary>Type and color of piece being dropped</summary>
    public PieceSnapshot Piece { get; private set; }
    /// <summary>Position on the board to drop piece to</summary>
    public Position To { get; private set; }
    /// <summary>Gets the color of player who made the move</summary>
    public override PieceColor Who
    {
      get { return Piece.Color; }
    }
    /// <summary>Type of piece being dropped</summary>
    public IPieceType PieceType { get { return Piece.PieceType; } }

    /// <summary>ctor</summary>
    public DropMove(IPieceType pieceType, PieceColor color, Position to)
    {
      // TODO: Change signature to match Board.GetDropMove()
      Piece = new PieceSnapshot(pieceType, color);
      To = to;
    }

    /// <summary>Returns user friendly move representation (latin)</summary>
    public override string ToString()
    {
      return PieceType.Latin + "'" + To;
    }
  }
}