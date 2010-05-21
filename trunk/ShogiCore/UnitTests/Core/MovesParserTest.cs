using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.Notations;
using Yasc.ShogiCore.Primitives;

namespace ShogiCore.UnitTests.ShogiCore
{
  [TestClass]
  public class MovesParserTest
  {
    private Board _board;

    [TestInitialize]
    public void Init()
    {
      _board = new Board();

      _board.SetPiece((PieceType)"歩", PieceColor.White, "1c");
      _board.SetPiece((PieceType)"歩", PieceColor.White, "2c");
      _board.White.Hand.Add((PieceType)"歩");

      _board.SetPiece((PieceType)"歩", PieceColor.Black, "8g");
      _board.SetPiece((PieceType)"歩", PieceColor.Black, "9g");
      _board.Black.Hand.Add((PieceType)"歩");
    }

    [TestMethod]
    public void TestMoveToSting()
    {
      var b = new Board();
      Shogi.InitBoard(b);
      var move = b.GetMove("3c-3d", FormalNotation.Instance).First();
      var m = (UsualMove) move;
      Assert.AreEqual(new Position(2, 2), m.From);
      Assert.AreEqual(new Position(2, 3), m.To);
      Assert.IsFalse(m.IsPromoting);
      Assert.AreEqual("3c-3d", move.ToString());
    }
    [TestMethod]
    public void TestUsualMoveToString()
    {
      var b = new Board();
      Shogi.InitBoard(b);
      var move = b.GetMove("3c-3d+", FormalNotation.Instance).First();
      var m = (UsualMove)move;
      Assert.AreEqual(new Position(2, 2), m.From);
      Assert.AreEqual(new Position(2, 3), m.To);
      Assert.IsTrue(m.IsPromoting);
      Assert.AreEqual("3c-3d+", move.ToString());
    }
    [TestMethod]
    public void TestUsualMoveNoPropmotionToString()
    {
      var b = new Board();
      Shogi.InitBoard(b);
      var move = b.GetMove("3c-3d=", FormalNotation.Instance).First();
      var m = (UsualMove)move;
      Assert.AreEqual(new Position(2, 2), m.From);
      Assert.AreEqual(new Position(2, 3), m.To);
      Assert.IsFalse(m.IsPromoting);
      Assert.AreEqual("3c-3d", move.ToString());
    }
    [TestMethod]
    public void TestDropMoveToString()
    {
      var b = new Board();
      var move = b.GetMove("P'3d", FormalNotation.Instance).First();
      var m = (DropMove)move;
      Assert.AreEqual("歩", (string)m.PieceType);
      Assert.AreEqual(new Position(2, 3), m.To);
      Assert.AreEqual("P'3d", move.ToString());
    }
    [TestMethod]
    public void MovesOrderTest()
    {
      _board.IsMovesOrderMaintained = false;
      var m = (UsualMove)_board.GetMove("8g-8f", FormalNotation.Instance).First();
      Assert.AreEqual("8g", m.From);
      Assert.AreEqual("8f", m.To);
      Assert.AreEqual(_board.Black, _board.OneWhoMoves);
    }
    [TestMethod]
    public void ParseResignMoveTest()
    {
      var move = (ResignMove)_board.GetMove("resign", FormalNotation.Instance).First();
      Assert.AreEqual(_board.Black, move.Who);
    }
  }
}