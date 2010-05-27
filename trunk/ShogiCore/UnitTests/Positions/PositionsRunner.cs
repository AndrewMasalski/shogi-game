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
  public abstract class PositionsRunner
  {
    protected void Run()
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

    private void ValidateDiagram(BoardDiagram diagram, Board board)
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

    protected abstract void ValidateUsualMoves(IUsualMoves usualMoves, Board board);
    protected abstract void ValidateDropMoves(Board board, IDropMoves dropMoves);
    protected static void AreEquivalent<T>(IEnumerable<T> expected, IEnumerable<T> actual, Func<T, T, bool> comparer)
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
  }
}