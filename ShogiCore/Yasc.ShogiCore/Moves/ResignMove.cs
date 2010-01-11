using Yasc.ShogiCore.Snapshots;
using Yasc.ShogiCore.SnapShots;

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

    protected internal override void Make()
    {
      Board.GameResult = Who.Color == PieceColor.White ? ShogiGameResult.BlackWin : ShogiGameResult.WhiteWin;
    }

    protected override string GetValidationErrorMessage()
    {
      if (Who != Board.OneWhoMoves)
        return "It's " + Board.OneWhoMoves + "'s move now";
      
      return null;
    }

    public override MoveSnapshotBase Snapshot()
    {
      return new ResignMoveSnapshot(Who.Color);
    }
  }
}