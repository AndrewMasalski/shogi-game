using System;
using Yasc.ShogiCore.Primitives;

namespace Yasc.ShogiCore.Snapshots
{
  /// <summary>Lightweight representation of drop move (as opposing to <see cref="UsualMoveSnapshot"/></summary>
  [Serializable]
  public sealed class DropMoveSnapshot : MoveSnapshotBase
  {
    /// <summary>Piece with type and color being dropped</summary>
    public PieceSnapshot Piece { get; private set; }
    /// <summary>Position on the board to drop piece to</summary>
    public Position To { get; private set; }
    /// <summary>Gets the color of player who made the move</summary>
    public override PieceColor Who
    {
      get { return Piece.Color; }
    }

    /// <summary>ctor</summary>
    public DropMoveSnapshot(IPieceType pieceType, PieceColor color, Position to)
    {
      // TODO: Change signature to match Board.GetDropMove()
      Piece = new PieceSnapshot(pieceType, color);
      To = to;
    }
  }
}