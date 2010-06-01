using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.RulesVisualization;
using Yasc.ShogiCore.Core;

namespace ShogiCore.UnitTests.Positions
{
  [TestClass]
  public class GetAvailableMovesRunner : PositionsRunner
  {
    [TestMethod]
    public void RunDiagrams()
    {
      Run();
    }


    protected override void ValidateDropMoves(Board board, IDropMoves dropMoves)
    {
      board.SideOnMove = board.GetPlayer(dropMoves.For);
      var moves = board.GetAvailableMoves(dropMoves.Piece, dropMoves.For);
      var expected = from p in dropMoves.To
                     select board.GetDropMove(dropMoves.Piece, p, board.SideOnMove);
      AreEquivalent(expected, moves, (x, y) =>
                                     x.PieceType == y.PieceType && x.To == y.To && x.Who == y.Who);
    }
    protected override void ValidateUsualMoves(IUsualMoves usualMoves, Board board)
    {
      board.SideOnMove = board.GetPieceAt(usualMoves.From).Owner;

      var moves = board.GetAvailableMoves(usualMoves.From);
      var expected = from p in usualMoves.To
                     select board.GetUsualMove(usualMoves.From, p.Position, p.Promotion);

      AreEquivalent(expected, moves, (x, y) =>
                                     x.From == y.From && x.To == y.To && x.IsPromoting == y.IsPromoting);
    }
  }
}