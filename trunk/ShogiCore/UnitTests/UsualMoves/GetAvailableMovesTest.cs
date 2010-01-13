using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.ShogiCore;
using Yasc.RulesVisualization;
using Yasc.ShogiCore;
using Yasc.Utils;

namespace UnitTests.UsualMoves
{
  [TestClass]
  public class GetAvailableMovesTest
  {
    [TestMethod]
    public void RunDiagrams()
    {
      var assembly = typeof(PieceTest).Assembly;
      foreach (var uri in assembly.GetBamlUris("Positions"))
      {
        Console.WriteLine("start: " + uri);
        var page = (Page)Application.LoadComponent(uri);
        var diagram = (BoardDiagram)page.Content;
        ValidateDiagram(diagram, diagram.Board);
      }
    }

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
      board.OneWhoMoves = board[dropMoves.For];
      var moves = board.GetAvailableMoves(dropMoves.Piece, dropMoves.For);
      var expected = from p in dropMoves.To
                     select board.GetDropMove(dropMoves.Piece, p, board.OneWhoMoves);
      AreEquivalent(expected, moves, (x, y) =>
        x.PieceType == y.PieceType && x.To == y.To && x.Who == y.Who);
    }
    private static void AreEquivalent<T>(IEnumerable<T> expected, IEnumerable<T> actual, Func<T, T, bool> comparer)
    {
      var e = expected.ToList();
      var a = actual.ToList();

      Assert.AreEqual(e.Count, a.Count);
      foreach (var x in a)
        Assert.AreEqual(1, e.RemoveAll(y => comparer(x, y)));
    }

    private static void ValidateUsualMoves(IUsualMoves usualMoves, Board board)
    {
      board.OneWhoMoves = board[usualMoves.From].Owner;

      var moves = board.GetAvailableMoves(usualMoves.From);
      var expected = from p in usualMoves.To
                     select board.GetUsualMove(usualMoves.From, p.Position, p.Promotion);

      AreEquivalent(expected, moves, (x, y) =>
        x.From == y.From && x.To == y.To && x.IsPromoting == y.IsPromoting);
    }
  }
}