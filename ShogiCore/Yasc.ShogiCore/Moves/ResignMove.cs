using Yasc.ShogiCore.Snapshots;

namespace Yasc.ShogiCore.Moves
{
  /// <summary>Represents resign move</summary>
  public sealed class ResignMove : MoveBase
  {
    /// <summary>ctor</summary>
    public ResignMove(Board board, Player who) 
      : base(board, who)
    {
    }

    /// <summary>Override to apply move to the <see cref="MoveBase.Board"/></summary>
    protected internal override void Make()
    {
      Board.GameResult = Who.Color == PieceColor.White ? ShogiGameResult.BlackWin : ShogiGameResult.WhiteWin;
    }

    /// <summary>Override to get validation error message or null if move is valid</summary>
    protected override string GetValidationErrorMessage()
    {
      if (Who != Board.OneWhoMoves)
        return "It's " + Board.OneWhoMoves + "'s move now";
      
      return null;
    }

    /// <summary>Gets snapshot of the move</summary>
    public override MoveSnapshotBase Snapshot()
    {
      return new ResignMoveSnapshot(Who.Color);
    }
  }
}