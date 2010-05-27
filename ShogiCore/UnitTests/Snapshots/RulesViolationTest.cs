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
        new[] { P("1c", PieceColor.Black, PT.歩) },
        blackHand: new[] { PT.歩, PT.香, PT.桂 });
    }

    [TestMethod]
    public void CantDropTwoPawnsToTheSameColumn()
    {
      var move = _board.ValidateDropMove(
        new DropMove(PT.歩, PieceColor.Black, Position.Parse("1d")));
      Assert.AreEqual(RulesViolation.TwoPawnsOnTheSameFile, move);
    }
    [TestMethod]
    public void CanDropPieceToFreeCellOnly()
    {
      var move = _board.ValidateDropMove(
        new DropMove(PT.歩, PieceColor.Black, Position.Parse("1c")));
      Assert.AreEqual(RulesViolation.DropToOccupiedCell, move);
    }
    [TestMethod]
    public void CantDropPawnToTheLastLine()
    {
      var move = _board.ValidateDropMove(
        new DropMove(PT.歩, PieceColor.Black, Position.Parse("1a")));
      Assert.AreEqual(RulesViolation.DropToLastLines, move);
    }
    [TestMethod]
    public void CantDropLanceToTheLastLine()
    {
      var move = _board.ValidateDropMove(
        new DropMove(PT.香, PieceColor.Black, Position.Parse("1a")));
      Assert.AreEqual(RulesViolation.DropToLastLines, move);
    }
    [TestMethod]
    public void CantDropKnightToTheLastLines()
    {
      var move = _board.ValidateDropMove(
        new DropMove(PT.桂, PieceColor.Black, Position.Parse("1a")));
      Assert.AreEqual(RulesViolation.DropToLastLines, move);
    }
    [TestMethod]
    public void CantDropPawnToMateTheOpponent()
    {
      var board = new BoardSnapshot(PieceColor.White,
        new[]
          {
            P("1i", PieceColor.Black, PT.玉),
            P("1g", PieceColor.Black, PT.歩),
            P("2h", PieceColor.White, PT.飛),
            P("3g", PieceColor.White, PT.金),
          },
        new[] { PT.歩 });

      var move = board.ValidateDropMove(
        new DropMove(PT.歩, PieceColor.White, Position.Parse("1h")));
      Assert.AreEqual(RulesViolation.DropPawnToMate, move);
    }

    #region ' Implementation '

    private static Tuple<Position, PieceSnapshot> P(string position, PieceColor color, IPieceType pieceType)
    {
      return Tuple.Create(Position.Parse(position), new PieceSnapshot(pieceType, color));
    }

    #endregion

  }
}