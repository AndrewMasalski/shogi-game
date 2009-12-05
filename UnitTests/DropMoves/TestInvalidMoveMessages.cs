using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;

namespace UnitTests.DropMoves
{
  [TestClass, NUnit.Framework.TestFixture]
  public class TestInvalidMoveMessages
  {
    private Board _board;

    [TestInitialize, NUnit.Framework.SetUp]
    public void Init()
    {
      _board = new Board();
    }

    [TestMethod, NUnit.Framework.Test]
    public void CantDropTwoPawnsToTheSameColumn()
    {
      _board["1c"] = new Piece(_board.White, "歩");
      _board.White.Hand.Add(new Piece(_board.White, "歩"));
      var move = _board.GetDropMove("歩", "1d", _board.OneWhoMoves);
      Assert.AreEqual("Can't drop 歩 to the column 1 " +
                      "because it already has one 歩", move.ErrorMessage);
    }

    [TestMethod, NUnit.Framework.Test]
    public void CanDropPieceToFreeCellOnly() 
    {
      _board["1c"] = new Piece(_board.White, "歩");
      _board.White.Hand.Add(new Piece(_board.White, "歩"));
      var move = _board.GetDropMove("歩", "1c", _board.OneWhoMoves);
      Assert.AreEqual("Can drop piece to free cell only", move.ErrorMessage);
    }

    [TestMethod, NUnit.Framework.Test]
    public void CantDropPawnToTheLastLine() 
    {
      _board.White.Hand.Add(new Piece(_board.White, "歩"));
      var move = _board.GetDropMove("歩", "1i", _board.OneWhoMoves);
      Assert.AreEqual("Can't drop 歩 to the last line", move.ErrorMessage);
    }

    [TestMethod, NUnit.Framework.Test]
    public void CantDropLanceToTheLastLine() 
    {
      _board.White.Hand.Add(new Piece(_board.White, "香"));
      var move = _board.GetDropMove("香", "1i", _board.OneWhoMoves);
      Assert.AreEqual("Can't drop 香 to the last line", move.ErrorMessage);
    }

    [TestMethod, NUnit.Framework.Test]
    public void CantDropKnightToTheLastLines()
    {
      _board.White.Hand.Add(new Piece(_board.White, "桂"));
      var move = _board.GetDropMove("桂", "1i", _board.OneWhoMoves);
      Assert.AreEqual("Can't drop 桂 to the last two lines", move.ErrorMessage);
    }

    [TestMethod, NUnit.Framework.Test]
    public void CantDropPawnToMateTheOpponent()
    {
      _board["1i"] = new Piece(_board.Black, "玉");
      _board["1g"] = new Piece(_board.Black, "歩");
      _board["2h"] = new Piece(_board.White, "飛");
      _board["3g"] = new Piece(_board.White, "金");
      _board.White.Hand.Add(new Piece(_board.White, "歩"));

      var move = _board.GetDropMove("歩", "1h", _board.White);
      Assert.AreEqual("Can't drop 歩 to mate the opponent", move.ErrorMessage);
    }
  }
}