using Yasc.ShogiCore.SnapShots;

namespace Yasc.ShogiCore.Moves
{
  public class DropMove : MoveBase
  {
    public PieceType PieceType { get; private set; }
    public Position To { get; private set; }

    private DropMove(Board board, PieceType pieceType, Position to, Player who) 
      : base(board, who)
    {
      PieceType = pieceType;
      To = to;
    }
    public static DropMove Create(Board board, PieceType pieceType, Position to, Player who)
    {
      var res = new DropMove(board, pieceType, to, who);
      res.Validate();
      return res;
    }
    public static DropMove Create(Board board, DropMoveSnapshot snapshot)
    {
      var res = new DropMove(board, snapshot.Piece.Type,
        snapshot.To, board[snapshot.Piece.Color]);
      res.Validate();
      return res;
    }
    protected internal override void Make()
    {
      Piece piece = Who.GetPieceFromHandByType(PieceType);
      Who.Hand.Remove(piece);
      Board.SetPiece(To, piece);
    }

    protected override string GetErrorMessage()
    {
      return BoardSnapshot.ValidateDropMove(new DropMoveSnapshot(this));
    }

    public override MoveSnapshotBase Shanpshot()
    {
      return new DropMoveSnapshot(this);
    }

    public override string ToString()
    {
      return PieceType.Latin + "'" + To;
    }
  }
}