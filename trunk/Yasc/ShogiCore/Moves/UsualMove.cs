using System;
using Yasc.ShogiCore.Snapshots;

namespace Yasc.ShogiCore.Moves
{
  public class UsualMove : MoveBase
  {
    public Position From { get; private set; }
    public Position To { get; private set; }
    public Piece MovingPiece { get; private set; }
    public Piece TakenPiece { get; private set; }
    public bool IsPromoting { get; private set; }

    public override MoveSnapshotBase Snapshot()
    {
      return new UsualMoveSnapshot(this);
    }
    public override string ToString()
    {
      return From + "-" + To + (IsPromoting ? "+" : "");
    }

    #region ' Construction '

    private UsualMove(Board board, Position from, Position to, bool isPromoting)
      : base(board, board.OneWhoMoves)
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

    #endregion

    protected override string GetErrorMessage()
    {
      return BoardSnapshot.ValidateUsualMove(new UsualMoveSnapshot(this));
    }
    protected internal override void Make()
    {
      var piece = Board[From];
      Board.ResetPiece(From);
      if (IsPromoting) piece.IsPromoted = true;
      var targetPiece = Board[To];
      if (targetPiece != null)
      {
        Board.ResetPiece(To);
        Who.Hand.Add(targetPiece);
      }
      Board.SetPiece(To, piece, Who);
    }
  }
}