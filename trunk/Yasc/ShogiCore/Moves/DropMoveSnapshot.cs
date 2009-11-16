using System;
using Yasc.ShogiCore.Utils;

namespace Yasc.ShogiCore.Moves
{
  [Serializable]
  public class DropMoveSnapshot : MoveSnapshotBase
  {
    public PieceSnapshot Piece { get; private set; }
    public Position To { get; private set; }

    public DropMoveSnapshot(DropMove move)
    {
      Piece = new PieceSnapshot(move.PieceType, move.Who.Color);
      To = move.To;
    }

    public DropMoveSnapshot(PieceSnapshot piece, Position to)
    {
      Piece = piece;
      To = to;
    }

    public override MoveBase AsRealMove(Board board)
    {
      return board.GetDropMove(Piece.Type, To, board[Piece.Color]);
    }
  }
}