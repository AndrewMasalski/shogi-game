using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.Notations;
using Yasc.ShogiCore.PieceSets;
using Yasc.ShogiCore.Primitives;
using Yasc.ShogiCore.Snapshots;

namespace ShogiCore.UnitTests.Notations
{
  [TestClass]
  public class MovesParserTest
  {
    private Board _board;

    [TestInitialize]
    public void Init()
    {
      _board = new Board(new StandardPieceSet());

      _board.SetPiece(PT.歩, "1c", PieceColor.White);
      _board.SetPiece(PT.歩, "2c", PieceColor.White);
      _board.White.Hand.Add(PT.歩);

      _board.SetPiece(PT.歩, "8g", PieceColor.Black);
      _board.SetPiece(PT.歩, "9g", PieceColor.Black);
      _board.Black.Hand.Add(PT.歩);
    }

    [TestMethod]
    public void TestMoveToSting()
    {
      var b = new Board(new StandardPieceSet());
      b.LoadSnapshot(BoardSnapshot.InitialPosition);
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
      var b = new Board(new StandardPieceSet());
      b.LoadSnapshot(BoardSnapshot.InitialPosition);
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
      var b = new Board(new StandardPieceSet());
      b.LoadSnapshot(BoardSnapshot.InitialPosition);
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
      var b = new Board(new StandardPieceSet());
      var move = b.GetMove("P'3d", FormalNotation.Instance).First();
      var m = (DropMove)move;
      Assert.AreEqual("歩", m.PieceType.ToString());
      Assert.AreEqual(new Position(2, 3), m.To);
      Assert.AreEqual("P'3d", move.ToString());
    }
    [TestMethod]
    public void MovesOrderTest()
    {
      _board.IsMovesOrderMaintained = false;
      var m = (UsualMove)_board.GetMove("8g-8f", FormalNotation.Instance).First();
      Assert.AreEqual("8g", m.From.ToString());
      Assert.AreEqual("8f", m.To.ToString());
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