using Yasc.ShogiCore.Moves.Validation;
using Yasc.ShogiCore.Utils;

namespace Yasc.ShogiCore.Moves
{
  public class UsualMove : MoveBase
  {
    public Position From { get; private set; }
    public Position To { get; private set; }
    public Piece MovingPiece { get; private set; }
    public Piece TakenPiece { get; private set; }
    public bool IsPromoting { get; private set; }

    private UsualMove(Board board, Position from, Position to, bool isPromoting)
      : base(board)
    {
      From = from;
      To = to;
      IsPromoting = isPromoting;
      MovingPiece = board[from];
      TakenPiece = board[to];
    }
    public static UsualMove Create(Board board, Position from, Position to, bool isPromoting)
    {
      var res = new UsualMove(board, from, to, isPromoting);
      res.Validate();
      return res;
    }
    public static UsualMove Create(Board board, UsualMoveSnapshot snapshot)
    {
      var res = new UsualMove(board, snapshot.From, snapshot.To, snapshot.IsPromoting);
      res.Validate();
      return res;
    }
    protected internal override void Make()
    {
      if (IsPromoting) Board[From].IsPromoted = true;
      if (Board[To] != null)
        Who.Hand.Add(Board[To]);
      Board[To] = Board[From];
      Board[From] = null;
    }

    protected override string GetErrorMessage()
    {
      return UsualMovesValidator.Check1(BoardSnapshot, new UsualMoveSnapshot(this));
    }

    public override string ToString()
    {
      return From + "-" + To + (IsPromoting ? "+" : "");
    }
  }
}