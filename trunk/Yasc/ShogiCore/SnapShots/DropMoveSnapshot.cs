using System;
using Yasc.ShogiCore.Moves;

namespace Yasc.ShogiCore.SnapShots
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

    protected override MoveBase AsRealMoveCore(Board board)
    {
      return AsRealMove(board);
    }
    public new DropMove AsRealMove(Board board)
    {
      return board.GetDropMove(Piece.Type, To, board[Piece.Color]);
    }

    public override PieceColor GetColor(BoardSnapshot snapshot)
    {
      return Piece.Color;
    }
  }
}