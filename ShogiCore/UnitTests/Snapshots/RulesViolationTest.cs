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
      var move = _board.ValidateDropMove(
        new DropMove(PT.歩.Black, Position.Parse("1d")));
      Assert.AreEqual(RulesViolation.TwoPawnsOnTheSameFile, move);
    }
    [TestMethod]
    public void CanDropPieceToFreeCellOnly()
    {
      var move = _board.ValidateDropMove(
        new DropMove(PT.歩.Black, Position.Parse("1c")));
      Assert.AreEqual(RulesViolation.DropToOccupiedCell, move);
    }
    [TestMethod]
    public void CantDropPawnToTheLastLine()
    {
      var move = _board.ValidateDropMove(
        new DropMove(PT.歩.Black, Position.Parse("1a")));
      Assert.AreEqual(RulesViolation.DropToLastLines, move);
    }
    [TestMethod]
    public void CantDropLanceToTheLastLine()
    {
      var move = _board.ValidateDropMove(
        new DropMove(PT.香.Black, Position.Parse("1a")));
      Assert.AreEqual(RulesViolation.DropToLastLines, move);
    }
    [TestMethod]
    public void CantDropKnightToTheLastLines()
    {
      var move = _board.ValidateDropMove(
        new DropMove(PT.桂.Black, Position.Parse("1a")));
      Assert.AreEqual(RulesViolation.DropToLastLines, move);
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

      var move = board.ValidateDropMove(
        new DropMove(PT.歩.White, Position.Parse("1h")));
      Assert.AreEqual(RulesViolation.DropPawnToMate, move);
    }

    #region ' Implementation '

    private static Tuple<Position, IColoredPiece> P(string position, IColoredPiece coloredPiece)
    {
      return Tuple.Create(Position.Parse(position), coloredPiece);
    }

    #endregion

  }
}