using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore.Primitives;
using Yasc.ShogiCore.Snapshots;

namespace ShogiCore.UnitTests.Snapshots
{
  [TestClass]
  public class RulesViolationTest
  {
    private BoardSnapshot _board;

    [TestInitialize]
    public void Init()
    {
      _board = new BoardSnapshot(PieceColor.Black,
        new[] { P("1c", PT.歩.Black) },
        blackHand: new[] { PT.歩, PT.香, PT.桂 });
    }

    [TestMethod]
    public void CantDropTwoPawnsToTheSameColumn()
    {
      var move = new DropMove(_board, PT.歩.Black, Position.Parse("1d"));
      Assert.AreEqual(RulesViolation.TwoPawnsOnTheSameFile, move.RulesViolation);
    }
    [TestMethod]
    public void CanDropPieceToFreeCellOnly()
    {
      var move = new DropMove(_board, PT.歩.Black, Position.Parse("1c"));
      Assert.AreEqual(RulesViolation.DropToOccupiedCell, move.RulesViolation);
    }
    [TestMethod]
    public void CantDropPawnToTheLastLine()
    {
      var move = new DropMove(_board, PT.歩.Black, Position.Parse("1a"));
      Assert.AreEqual(RulesViolation.DropToLastLines, move.RulesViolation);
    }
    [TestMethod]
    public void CantDropLanceToTheLastLine()
    {
      var move = new DropMove(_board, PT.香.Black, Position.Parse("1a"));
      Assert.AreEqual(RulesViolation.DropToLastLines, move.RulesViolation);
    }
    [TestMethod]
    public void CantDropKnightToTheLastLines()
    {
      var move = new DropMove(_board, PT.桂.Black, Position.Parse("1a"));
      Assert.AreEqual(RulesViolation.DropToLastLines, move.RulesViolation);
    }
    [TestMethod]
    public void CantDropPawnToMateTheOpponent()
    {
      var board = new BoardSnapshot(PieceColor.White,
        new[]
          {
            P("1i", PT.玉.Black),
            P("1g", PT.歩.Black),
            P("2h", PT.飛.White),
            P("3g", PT.金.White),
          },
        new[] { PT.歩 });

      var move = new DropMove(board, PT.歩.White, Position.Parse("1h"));
      Assert.AreEqual(RulesViolation.DropPawnToMate, move.RulesViolation);
    }

    #region ' Implementation '

    private static Tuple<Position, IColoredPiece> P(string position, IColoredPiece coloredPiece)
    {
      return Tuple.Create(Position.Parse(position), coloredPiece);
    }

    #endregion

  }
}