using System;
using Yasc.ShogiCore.Primitives;

namespace Yasc.ShogiCore.Snapshots
{
  /// <summary>Represents lightweight snapshot of resign move</summary>
  [Serializable]
  public sealed class ResignMove : Move
  {
    private readonly PieceColor _who;

    public override MoveType MoveType
    {
      get { return MoveType.Resign; }
    }

    /// <summary>Gets the color of player who made the move</summary>
    public override PieceColor Who
    {
      get { return _who; }
    }

    internal override RulesViolation Validate()
    {
      return Who != BoardSnapshotBefore.SideOnMove
               ? RulesViolation.WrongSideToMove
               : RulesViolation.NoViolations;
    }

    internal override void Apply(BoardSnapshot board)
    {
      board.GameState = Who == PieceColor.White ? 
        ShogiGameState.BlackWin : ShogiGameState.WhiteWin;
    }

    /// <summary>ctor</summary>
    public ResignMove(BoardSnapshot boardSnapshot, PieceColor who)
      : base(boardSnapshot)
    {
      _who = who;
    }

    /// <summary>Returns user friendly move representation (latin)</summary>
    public override string ToString()
    {
      return "Resign";
    }
  }
}