using System;
using Yasc.ShogiCore.Moves;

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

    /// <summary>ctor</summary>
    public DropMoveSnapshot(DropMove move)
    {
      Piece = new PieceSnapshot(move.PieceType, move.Who.Color);
      To = move.To;
    }
    /// <summary>ctor</summary>
    public DropMoveSnapshot(PieceSnapshot piece, Position to)
    {
      Piece = piece;
      To = to;
    }
    public override PieceColor GetColor(BoardSnapshot snapshot)
    {
      return Piece.Color;
    }
    public new DropMove AsRealMove(Board board)
    {
      return board.GetDropMove(Piece.PieceType, To, board[Piece.Color]);
    }
    
    protected override MoveBase AsRealMoveCore(Board board)
    {
      return AsRealMove(board);
    }
  }
}