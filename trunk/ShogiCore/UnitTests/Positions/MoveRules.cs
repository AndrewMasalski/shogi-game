using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShogiCore.UnitTests.Core;
using Yasc.RulesVisualization;
using Yasc.ShogiCore.Core;
using Yasc.Utils;

namespace ShogiCore.UnitTests.Positions
{
  [TestClass]
  public class MoveRules
  {
    [TestMethod]
    public void RunDiagrams()
    {
      var assembly = typeof(PieceTest).Assembly;
      var bamlUris = assembly.GetBamlUris("Positions", null).ToList();
      Assert.AreNotEqual(0, bamlUris.Count, "No test positions found!");
      foreach (var uri in bamlUris)
      {
        Console.WriteLine("start: " + uri);
        var page = (Page)Application.LoadComponent(uri);
        var diagram = (BoardDiagram)page.Content;
        ValidateDiagram(diagram, diagram.Board);
      }
    }

    #region ' Implementation '

    private static void ValidateDiagram(BoardDiagram diagram, Board board)
    {
      foreach (var moves in diagram.Moves)
      {
        var usualMoves = moves as IUsualMoves;
        if (usualMoves != null)
        {
          ValidateUsualMoves(usualMoves, board);
        }
        var dropMoves = moves as IDropMoves;
        if (dropMoves != null)
        {
          ValidateDropMoves(board, dropMoves);
        }
      }
    }
    private static void ValidateDropMoves(Board board, IDropMoves dropMoves)
    {
      board.OneWhoMoves = board.GetPlayer(dropMoves.For);
      var moves = board.GetAvailableMoves(dropMoves.Piece, dropMoves.For);
      var expected = from p in dropMoves.To
                     select board.GetDropMove(dropMoves.Piece, p, board.OneWhoMoves);
      AreEquivalent(expected, moves, (x, y) =>
         x.PieceType == y.PieceType && x.To == y.To && x.Who == y.Who);
    }
    private static void AreEquivalent<T>(IEnumerable<T> expected, IEnumerable<T> actual, Func<T, T, bool> comparer)
    {
      var expectedList = expected.ToList();
      var actualList = actual.ToList();

      Assert.AreEqual(expectedList.Count, actualList.Count);
      foreach (var actualElement in actualList)
      {
        var actualElementCopy = actualElement;
        Assert.AreEqual(1, 
                        expectedList.RemoveAll(expectedElement => 
                                               comparer(actualElementCopy, expectedElement)));
      }
    }
    private static void ValidateUsualMoves(IUsualMoves usualMoves, Board board)
    {
      board.OneWhoMoves = board.GetPieceAt(usualMoves.From).Owner;

      var moves = board.GetAvailableMoves(usualMoves.From);
      var expected = from p in usualMoves.To
                     select board.GetUsualMove(usualMoves.From, p.Position, p.Promotion);

      AreEquivalent(expected, moves, (x, y) =>
                                     x.From == y.From && x.To == y.To && x.IsPromoting == y.IsPromoting);
    }

    #endregion
  }
}