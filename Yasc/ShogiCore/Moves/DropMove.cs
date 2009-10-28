using Yasc.ShogiCore.Moves.Validation;
using Yasc.ShogiCore.Utils;

namespace Yasc.ShogiCore.Moves
{
  public class DropMove : MoveBase
  {
    public PieceType PieceType { get; private set; }
    public Position To { get; private set; }

    private DropMove(Board board, PieceType pieceType, Position to) 
      : base(board)
    {
      PieceType = pieceType;
      To = to;
    }
    public static DropMove Create(Board board, PieceType pieceType, Position to)
    {
      var res = new DropMove(board, pieceType, to);
      res.Validate();
      return res;
    }

    protected internal override void Make()
    {
      Piece piece = Who.GetPieceFromHandByType(PieceType);
      Who.Hand.Remove(piece);
      Board[To] = piece;
    }

    protected override string GetErrorMessage()
    {
      return DropMoveValidator.GetError(BoardSnapshot, new DropMoveSnapshot(this));
    }

    public override string ToString()
    {
      return PieceType.Latin + "'" + To;
    }
  }
}