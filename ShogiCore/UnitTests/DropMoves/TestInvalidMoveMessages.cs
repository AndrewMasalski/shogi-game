using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;

namespace ShogiCore.UnitTests.DropMoves
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
      _board.SetPiece("歩", _board.Black, "1c");
      _board.Black.AddToHand("歩");
      var move = _board.GetDropMove("歩", "1d", _board.OneWhoMoves);
      Assert.AreEqual("Can't drop 歩 to the column 1 " +
                      "because it already has one 歩", move.ErrorMessage);
    }

    [TestMethod]
    public void CanDropPieceToFreeCellOnly()
    {
      _board.SetPiece("歩", _board.Black, "1c");
      _board.Black.AddToHand("歩");
      var move = _board.GetDropMove("歩", "1c", _board.OneWhoMoves);
      Assert.AreEqual("Can drop piece to free cell only", move.ErrorMessage);
    }

    [TestMethod]
    public void CantDropPawnToTheLastLine()
    {
      _board.Black.AddToHand("歩");
      var move = _board.GetDropMove("歩", "1a", _board.OneWhoMoves);
      Assert.AreEqual("Can't drop 歩 to the last line", move.ErrorMessage);
    }

    [TestMethod]
    public void CantDropLanceToTheLastLine()
    {
      _board.Black.AddToHand("香");
      var move = _board.GetDropMove("香", "1a", _board.OneWhoMoves);
      Assert.AreEqual("Can't drop 香 to the last line", move.ErrorMessage);
    }

    [TestMethod]
    public void CantDropKnightToTheLastLines()
    {
      _board.Black.AddToHand("桂");
      var move = _board.GetDropMove("桂", "1a", _board.OneWhoMoves);
      Assert.AreEqual("Can't drop 桂 to the last two lines", move.ErrorMessage);
    }

    [TestMethod]
    public void CantDropPawnToMateTheOpponent()
    {
      _board.OneWhoMoves = _board.White;
      _board.SetPiece("玉", _board.Black, "1i");
      _board.SetPiece("歩", _board.Black, "1g");
      _board.SetPiece("飛", _board.White, "2h");
      _board.SetPiece("金", _board.White, "3g");
      _board.White.AddToHand("歩");

      var move = _board.GetDropMove("歩", "1h", _board.White);
      Assert.AreEqual("Can't drop 歩 to mate the opponent", move.ErrorMessage);
    }
  }
}