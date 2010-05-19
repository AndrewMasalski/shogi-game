using Yasc.ShogiCore.Snapshots;

namespace Yasc.ShogiCore.Moves
{
  /// <summary>Represents drop move (as opposing to <see cref="UsualMove"/>)</summary>
  public sealed class DropMove : MoveBase
  {
    /// <summary>Droping piece type</summary>
    public PieceType PieceType { get; private set; }
    /// <summary>Position to drop piece to</summary>
    public Position To { get; private set; }

    private DropMove(Board board, PieceType pieceType, Position to, Player who) 
      : base(board, who)
    {
      PieceType = pieceType;
      To = to;
    }
    /// <summary>Creates an instance of <see cref="DropMove"/> 
    ///   from type and position and validates it immediately</summary>
    public static DropMove Create(Board board, PieceType pieceType, Position to, Player who)
    {
      var res = new DropMove(board, pieceType, to, who);
      res.Validate();
      return res;
    }
    /// <summary>Creates an instance of <see cref="DropMove"/> 
    ///   from snapshot and validates it immediately</summary>
    public static DropMove Create(Board board, DropMoveSnapshot snapshot)
    {
      var res = new DropMove(board, snapshot.Piece.PieceType,
        snapshot.To, board[snapshot.Piece.Color]);
      res.Validate();
      return res;
    }
    /// <summary>Applies move to the <see cref="MoveBase.Board"/></summary>
    protected internal override void Make()
    {
      var piece = Who.Hand.GetByType(PieceType);
      Who.Hand.Remove(piece);
      Board.SetPiece(piece, Who, To);
    }

    /// <summary>Override to get validation error message or null if move is valid</summary>
    protected override string GetValidationErrorMessage()
    {
      return BoardSnapshot.ValidateDropMove(
        new DropMoveSnapshot(PieceType, Who.Color, To));
    }
    /// <summary>Gets snapshot of the <see cref="DropMove"/></summary>
    public override MoveSnapshotBase Snapshot()
    {
      return new DropMoveSnapshot(PieceType, Who.Color, To);
    }

    /// <summary>Returns user friendly move representation (latin)</summary>
    public override string ToString()
    {
      return PieceType.Latin + "'" + To;
    }
  }
}