using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.RulesVisualization;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.PieceSets;
using Yasc.ShogiCore.Primitives;
using Yasc.ShogiCore.Snapshots;
using Yasc.Utils;

namespace ShogiCore.UnitTests.Core
{
  [TestClass]
  public class UsualMoveTest
  {
    private Board _board;

    [TestInitialize]
    public void Init()
    {
      _board = new Board(new StandardPieceSet());
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
    private static void ValidateUsualMoves(IUsualMoves usualMoves, Board board)
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

    #endregion

    [TestMethod]
    public void ValidMoveWithoutTakingPieceTest()
    {
      _board.LoadSnapshot(BoardSnapshot.InitialPosition);
      var move = _board.GetUsualMove("9g", "9f");
      _board.MakeWrapedMove(move);
      Assert.IsNull(_board.GetPieceAt("9g"));
      Assert.AreEqual(PT.歩, _board.GetPieceAt("9f").PieceType);
    }
    [TestMethod]
    public void ValidMoveWithTakingPieceTest()
    {
      _board.LoadSnapshot(BoardSnapshot.InitialPosition);

      _board.OneWhoMoves = _board.White; _board.MakeWrapedMove(_board.GetUsualMove("9c", "9d"));
      _board.OneWhoMoves = _board.White; _board.MakeWrapedMove(_board.GetUsualMove("9d", "9e"));
      _board.OneWhoMoves = _board.White; _board.MakeWrapedMove(_board.GetUsualMove("9e", "9f"));
      _board.OneWhoMoves = _board.White; _board.MakeWrapedMove(_board.GetUsualMove("9f", "9g"));

      Assert.IsNull(_board.GetPieceAt("9c"));
      Assert.IsNull(_board.GetPieceAt("9d"));
      Assert.IsNull(_board.GetPieceAt("9e"));
      Assert.IsNull(_board.GetPieceAt("9f"));

      Assert.AreEqual(PT.歩, _board.GetPieceAt("9g").PieceType);
      Assert.AreEqual(_board.White, _board.GetPieceAt("9g").Owner);
      Assert.AreEqual(1, _board.White.Hand.Count);
      Assert.AreEqual(PT.歩, _board.White.Hand[0].PieceType);
      Assert.AreEqual(_board.White, _board.White.Hand[0].Owner);
    }
    [TestMethod, ExpectedException(typeof(InvalidMoveException))]
    public void InvalidMoveTest()
    {
      var badMove = _board.GetUsualMove("1c", "1d");
      _board.MakeWrapedMove(badMove);
    }
    [TestMethod]
    public void InvalidMoveMessageTest()
    {
      var badMove = _board.Wrap(_board.GetUsualMove("1c", "1d"));
      Assert.IsFalse(badMove.IsValid);
      Assert.AreEqual(RulesViolation.WrongPieceReference, badMove.RulesViolation);
    }
    [TestMethod, ExpectedException(typeof(InvalidMoveException))]
    public void GetUsualMoveFromEmptyCell()
    {
      var move = _board.Wrap(_board.GetUsualMove("3d", "3e"));

      Assert.IsFalse(move.IsValid);
      Assert.AreEqual(RulesViolation.WrongPieceReference, move.RulesViolation);
      _board.MakeMove(move);
    }
    [TestMethod]
    public void TestMoveOrder()
    {
      _board.LoadSnapshot(BoardSnapshot.InitialPosition);
      _board.MakeWrapedMove(_board.GetUsualMove("3g", "3f"));
      var move = _board.Wrap(_board.GetUsualMove("3f", "3e"));
      Assert.AreEqual(RulesViolation.WrongSideToMove, move.RulesViolation);
    }
  }
}