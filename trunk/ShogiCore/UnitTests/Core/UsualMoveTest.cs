using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.RulesVisualization;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.Primitives;
using Yasc.ShogiCore.Snapshots;
using Yasc.Utils;

namespace ShogiCore.UnitTests.ShogiCore.Moves
{
  [TestClass]
  public class UsualMoveTest
  {
    private Board _board;

    [TestInitialize]
    public void Init()
    {
      _board = new Board();
    }

    #region ' RunDiagrams '

    [TestMethod]
    public void RunDiagrams()
    {
      var assembly = typeof(PieceTest).Assembly;
      foreach (var uri in assembly.GetBamlUris("Positions"))
      {
        Console.WriteLine("start: " + uri);
        var page = (Page)Application.LoadComponent(uri);
        var diagram = (BoardDiagram)page.Content;
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
        var move = board.GetUsualMove(usualMoves.From, to.Position, to.Promotion);
        Assert.IsTrue(move.IsValid, move.ErrorMessage);
      }

      foreach (var to in usualMoves.NotTo)
      {
        var move = board.GetUsualMove(usualMoves.From, to.Position, to.Promotion);
        Assert.IsFalse(move.IsValid, "Move " + move + " is also valid");
      }
    }

    #endregion

    [TestMethod]
    public void ValidMoveWithoutTakingPieceTest()
    {
      _board.LoadSnapshot(BoardSnapshot.InitialPosition);
      var move = _board.GetUsualMove("9g", "9f", false);
      _board.MakeMove(move);
      Assert.IsNull(_board["9g"]);
      Assert.AreEqual(PieceType.歩, _board["9f"].PieceType);
    }
    [TestMethod]
    public void ValidMoveWithTakingPieceTest()
    {
      _board.LoadSnapshot(BoardSnapshot.InitialPosition);

      _board.OneWhoMoves = _board.White; _board.MakeMove(_board.GetUsualMove("9c", "9d", false));
      _board.OneWhoMoves = _board.White; _board.MakeMove(_board.GetUsualMove("9d", "9e", false));
      _board.OneWhoMoves = _board.White; _board.MakeMove(_board.GetUsualMove("9e", "9f", false));
      _board.OneWhoMoves = _board.White; _board.MakeMove(_board.GetUsualMove("9f", "9g", false));

      Assert.IsNull(_board["9c"]);
      Assert.IsNull(_board["9d"]);
      Assert.IsNull(_board["9e"]);
      Assert.IsNull(_board["9f"]);

      Assert.AreEqual(PieceType.歩, _board["9g"].PieceType);
      Assert.AreEqual(_board.White, _board["9g"].Owner);
      Assert.AreEqual(1, _board.White.Hand.Count);
      Assert.AreEqual(PieceType.歩, _board.White.Hand[0].PieceType);
      Assert.AreEqual(_board.White, _board.White.Hand[0].Owner);
    }
    [TestMethod, ExpectedException(typeof(InvalidMoveException))]
    public void InvalidMoveTest()
    {
      var badMove = _board.GetUsualMove("1c", "1d", false);
      _board.MakeMove(badMove);
    }
    [TestMethod]
    public void InvalidMoveMessageTest()
    {
      var badMove = _board.GetUsualMove("1c", "1d", false);
      Assert.IsFalse(badMove.IsValid);
      Assert.AreEqual("No piece at 1c", badMove.ErrorMessage);
    }
    [TestMethod, ExpectedException(typeof(InvalidMoveException))]
    public void GetUsualMoveFromEmptyCell()
    {
      var move = _board.GetUsualMove("3d", "3e", false);

      Assert.IsFalse(move.IsValid);
      Assert.AreEqual("No piece at 3d", move.ErrorMessage);
      _board.MakeMove(move);
    }
    [TestMethod]
    public void TestMoveOrder()
    {
      _board.LoadSnapshot(BoardSnapshot.InitialPosition);
      _board.MakeMove(_board.GetUsualMove("3g", "3f", false));
      Assert.AreEqual("It's White's move now",
        _board.GetUsualMove("3f", "3e", false).ErrorMessage);
    }
  }
}