using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.PieceSets;
using Yasc.ShogiCore.Primitives;
using Yasc.ShogiCore.Snapshots;

namespace ShogiCore.UnitTests.Core
{
  [TestClass]
  public class TestInvalidMoveMessages
  {
    private Board _board;

    [TestInitialize]
    public void Init()
    {
      _board = new Board(new StandardPieceSet());
    }

    [TestMethod]
    public void CantDropTwoPawnsToTheSameColumn()
    {
      _board.SetPiece(PT.歩, "1c", _board.Black);
      _board.Black.Hand.Add(PT.歩);
      var move = _board.Wrap(_board.GetDropMove(PT.歩, "1d", _board.OneWhoMoves));
      Assert.AreEqual(RulesViolation.TwoPawnsOnTheSameFile, move.RulesViolation);
    }

    [TestMethod]
    public void CanDropPieceToFreeCellOnly()
    {
      _board.SetPiece(PT.歩, "1c", _board.Black);
      _board.Black.Hand.Add(PT.歩);
      var move = _board.Wrap(_board.GetDropMove(PT.歩, "1c", _board.OneWhoMoves));
      Assert.AreEqual(RulesViolation.DropToOccupiedCell, move.RulesViolation);
    }

    [TestMethod]
    public void CantDropPawnToTheLastLine()
    {
      _board.Black.Hand.Add(PT.歩);
      var move = _board.Wrap(_board.GetDropMove(PT.歩, "1a", _board.OneWhoMoves));
      Assert.AreEqual(RulesViolation.DropToLastLines, move.RulesViolation);
    }

    [TestMethod]
    public void CantDropLanceToTheLastLine()
    {
      _board.Black.Hand.Add(PT.香);
      var move = _board.Wrap(_board.GetDropMove(PT.香, "1a", _board.OneWhoMoves));
      Assert.AreEqual(RulesViolation.DropToLastLines, move.RulesViolation);
    }

    [TestMethod]
    public void CantDropKnightToTheLastLines()
    {
      _board.Black.Hand.Add(PT.桂);
      var move = _board.Wrap(_board.GetDropMove(PT.桂, "1a", _board.OneWhoMoves));
      Assert.AreEqual(RulesViolation.DropToLastLines, move.RulesViolation);
    }

    [TestMethod]
    public void CantDropPawnToMateTheOpponent()
    {
      _board.OneWhoMoves = _board.White;
      _board.SetPiece(PT.玉, "1i", _board.Black);
      _board.SetPiece(PT.歩, "1g", _board.Black);
      _board.SetPiece(PT.飛, "2h", _board.White);
      _board.SetPiece(PT.金, "3g", _board.White);
      _board.White.Hand.Add(PT.歩);

      var move = _board.Wrap(_board.GetDropMove(PT.歩, "1h", _board.White));
      Assert.AreEqual(RulesViolation.DropPawnToMate, move.RulesViolation);
    }
  }
}