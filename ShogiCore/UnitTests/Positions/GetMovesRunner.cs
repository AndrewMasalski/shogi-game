using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.RulesVisualization;
using Yasc.ShogiCore.Core;

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
      board.OneWhoMoves = board.GetPlayer(dropMoves.For);
      foreach (var to in dropMoves.To)
      {
        var move = board.Wrap(board.GetDropMove(dropMoves.Piece, to, board.GetPlayer(dropMoves.For)));
        Assert.IsTrue(move.IsValid, move.RulesViolation.ToString());
      }

      foreach (var to in dropMoves.NotTo)
      {
        var move = board.Wrap(board.GetDropMove(dropMoves.Piece, to, board.GetPlayer(dropMoves.For)));
        Assert.IsFalse(move.IsValid, "Move " + move + " is also valid");
      }
    }
    protected override void ValidateUsualMoves(IUsualMoves usualMoves, Board board)
    {
      board.OneWhoMoves = board.GetPieceAt(usualMoves.From).Owner;
      foreach (var to in usualMoves.To)
      {
        var move = board.Wrap(board.GetUsualMove(usualMoves.From, to.Position, to.Promotion));
        Assert.IsTrue(move.IsValid, move.RulesViolation.ToString());
      }

      foreach (var to in usualMoves.NotTo)
      {
        var move = board.Wrap(board.GetUsualMove(usualMoves.From, to.Position, to.Promotion));
        Assert.IsFalse(move.IsValid, "Move " + move + " is also valid");
      }
    }

  }
}