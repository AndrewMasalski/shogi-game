using Yasc.ShogiCore.Moves.Validation;
using Yasc.ShogiCore.Utils;

namespace Yasc.ShogiCore.Moves
{
  public class DropMove : MoveBase
  {
    public Piece Piece { get; private set; }
    public Position To { get; private set; }

    private DropMove(Board board, Piece piece, Position to) 
      : base(board)
    {
      Piece = piece;
      To = to;
    }
    public static DropMove Create(Board board, Piece piece, Position to)
    {
      var res = new DropMove(board, piece, to);
      res.Validate();
      return res;
    }

    protected internal override void Make()
    {
      Who.Hand.Remove(Piece);
      Board[To] = Piece;
    }

    protected override string GetErrorMessage()
    {
      return DropMoveValidator.GetError(BoardSnapshot, new DropMoveSnapshot(this));
    }

    public override string ToString()
    {
      return Piece + "'" + To;
    }
  }
}