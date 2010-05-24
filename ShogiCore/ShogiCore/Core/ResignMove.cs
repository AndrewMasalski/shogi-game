using Yasc.ShogiCore.Primitives;
using Yasc.ShogiCore.Snapshots;

namespace Yasc.ShogiCore.Core
{
  /// <summary>Represents resign move</summary>
  public sealed class ResignMove : MoveBase
  {
    private RulesViolation _errorMessage;

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
    /// <summary>null if move is valid -or- explanation why it's not</summary>
    public override RulesViolation RulesViolation
    {
      get
      {
        return _errorMessage = _errorMessage == RulesViolation.HasntBeenChecked
          ? GetValidationErrorMessage()
          : _errorMessage;
      }
    }
    /// <summary>Override to get validation error message or null if move is valid</summary>
    private RulesViolation GetValidationErrorMessage()
    {
      return Who != Board.OneWhoMoves 
        ? RulesViolation.WrongSideToMove 
        : RulesViolation.NoViolations;
    }

    /// <summary>Gets snapshot of the move</summary>
    public override MoveSnapshotBase Snapshot()
    {
      return new ResignMoveSnapshot(Who.Color);
    }
  }
}