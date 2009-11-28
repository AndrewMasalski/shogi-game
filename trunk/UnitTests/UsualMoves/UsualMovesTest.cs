using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.RulesVisualization;
using Yasc.ShogiCore;

namespace UnitTests.UsualMoves
{
  [TestClass]
  public class UsualMovesTest
  {
    [TestMethod]
    public void RunDiagrams()
    {
      var assembly = typeof(PieceTest).Assembly;
      foreach (var uri in assembly.GetBamlUris("Positions"))
      {
        var page = (Page) Application.LoadComponent(uri);
        var diagram = (BoardDiagram)page.Content;
        Console.WriteLine(uri);
        var board = diagram.Board;
        ValidateDiagram(diagram, board);
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
      foreach (var to in dropMoves.To)
      {
        var move = board.GetDropMove(dropMoves.Piece, to, board[dropMoves.For]);
        Assert.IsTrue(move.IsValid, move.ErrorMessage);
      }

      foreach (var to in dropMoves.NotTo)
      {
        var move = board.GetDropMove(dropMoves.Piece, to, board[dropMoves.For]);
        Assert.IsFalse(move.IsValid, "Move " + move + " is also valid");
      }
    }
    private static void ValidateUsualMoves(IUsualMoves usualMoves, Board board)
    {
      board.OneWhoMoves = board[usualMoves.From].Owner;
      foreach (var to in usualMoves.To)
      {
        var move = board.GetUsualMove(usualMoves.From, to, false);
        Assert.IsTrue(move.IsValid, move.ErrorMessage);
      }

      foreach (var to in usualMoves.NotTo)
      {
        var move = board.GetUsualMove(usualMoves.From, to, false);
        Assert.IsFalse(move.IsValid, "Move " + move + " is also valid");
      }
    }
  }
}