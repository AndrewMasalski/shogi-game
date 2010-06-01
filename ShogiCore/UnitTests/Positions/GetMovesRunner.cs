using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.RulesVisualization;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.Snapshots;

namespace ShogiCore.UnitTests.Positions
{
  [TestClass]
  public class GetMovesRunner : PositionsRunner
  {
    [TestMethod]
    public void RunDiagrams()
    {
      Run();
    }

    protected override void ValidateDropMoves(Board board, IDropMoves dropMoves)
    {
      board.SideOnMove = board.GetPlayer(dropMoves.For);
      foreach (var to in dropMoves.To)
      {
        var move = board.GetDropMove(dropMoves.Piece, to, dropMoves.For);
        Assert.AreEqual(RulesViolation.NoViolations, move.RulesViolation);
      }

      foreach (var to in dropMoves.NotTo)
      {
        var move = board.GetDropMove(dropMoves.Piece, to, board.GetPlayer(dropMoves.For));
        Assert.AreNotEqual(RulesViolation.NoViolations, move.RulesViolation);
      }
    }
    protected override void ValidateUsualMoves(IUsualMoves usualMoves, Board board)
    {
      board.SideOnMove = board.GetPieceAt(usualMoves.From).Owner;
      foreach (var to in usualMoves.To)
      {
        var move = board.GetUsualMove(usualMoves.From, to.Position, to.Promotion);
        Assert.AreEqual(RulesViolation.NoViolations, move.RulesViolation);
      }

      foreach (var to in usualMoves.NotTo)
      {
        var move = board.GetUsualMove(usualMoves.From, to.Position, to.Promotion);
        Assert.AreNotEqual(RulesViolation.NoViolations, move.RulesViolation);
      }
    }

  }
}