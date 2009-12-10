using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;

namespace UnitTests.DropMoves
{
  [TestClass]
  public class TestInvalidMoveMessages
  {
    private Board _board;

    [TestInitialize]
    public void Init()
    {
      _board = new Board();
    }

    [TestMethod]
    public void CantDropTwoPawnsToTheSameColumn()
    {
      _board.SetPiece("1c", _board.White, "歩");
      _board.White.Hand.Add(_board.GetSparePiece("歩"));
      var move = _board.GetDropMove("歩", "1d", _board.OneWhoMoves);
      Assert.AreEqual("Can't drop 歩 to the column 1 " +
                      "because it already has one 歩", move.ErrorMessage);
    }

    [TestMethod]
    public void CanDropPieceToFreeCellOnly()
    {
      _board.SetPiece("1c", _board.White, "歩");
      _board.White.Hand.Add(_board.GetSparePiece("歩"));
      var move = _board.GetDropMove("歩", "1c", _board.OneWhoMoves);
      Assert.AreEqual("Can drop piece to free cell only", move.ErrorMessage);
    }

    [TestMethod]
    public void CantDropPawnToTheLastLine()
    {
      _board.White.Hand.Add(_board.GetSparePiece("歩"));
      var move = _board.GetDropMove("歩", "1i", _board.OneWhoMoves);
      Assert.AreEqual("Can't drop 歩 to the last line", move.ErrorMessage);
    }

    [TestMethod]
    public void CantDropLanceToTheLastLine()
    {
      _board.White.Hand.Add(_board.GetSparePiece("香"));
      var move = _board.GetDropMove("香", "1i", _board.OneWhoMoves);
      Assert.AreEqual("Can't drop 香 to the last line", move.ErrorMessage);
    }

    [TestMethod]
    public void CantDropKnightToTheLastLines()
    {
      _board.White.Hand.Add(_board.GetSparePiece("桂"));
      var move = _board.GetDropMove("桂", "1i", _board.OneWhoMoves);
      Assert.AreEqual("Can't drop 桂 to the last two lines", move.ErrorMessage);
    }

    [TestMethod]
    public void CantDropPawnToMateTheOpponent()
    {
      _board.SetPiece("1i", _board.Black, "玉");
      _board.SetPiece("1g", _board.Black, "歩");
      _board.SetPiece("2h", _board.White, "飛");
      _board.SetPiece("3g", _board.White, "金");
      _board.White.Hand.Add(_board.GetSparePiece("歩"));

      var move = _board.GetDropMove("歩", "1h", _board.White);
      Assert.AreEqual("Can't drop 歩 to mate the opponent", move.ErrorMessage);
    }
  }
}