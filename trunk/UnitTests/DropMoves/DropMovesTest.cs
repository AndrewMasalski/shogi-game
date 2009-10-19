using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Utils;

namespace UnitTests
{
  [TestClass]
  public class DropMovesTest
  {
    private Board _board;

    [TestInitialize]
    public void Init()
    {
      _board = new Board();
    }

    [TestMethod]
    public void SimplestTest()
    {
      Shogi.InititBoard(_board);
      var piece = _board["9a"];
      _board.Black.Hand.Add(piece);
      _board["9a"] = null;
      _board.OneWhoMoves = _board.Black;
      var move = _board.GetDropMove(piece, "9e");
      _board.MakeMove(move);
      Assert.AreSame(piece, _board["9e"]);
      Assert.AreSame(_board.Black, piece.Owner);
    }
    [TestMethod]
    public void MovesOrderMaintenanceFalse()
    {
      Shogi.InititBoard(_board);
      var piece = _board["9a"];
      _board.Black.Hand.Add(piece);
      _board["9a"] = null;
      _board.IsMovesOrderMaintained = false;
      var move = _board.GetDropMove(piece, "9e");
      _board.MakeMove(move);
      Assert.AreSame(piece, _board["9e"]);
      Assert.AreSame(_board.Black, piece.Owner);
    }
    [TestMethod]
    public void MovesOrderMaintenanceTrue()
    {
      Shogi.InititBoard(_board);
      var piece = _board["9a"];
      _board.Black.Hand.Add(piece);
      _board["9a"] = null;
      _board.IsMovesOrderMaintained = true;
      var move = _board.GetDropMove(piece, "9e");
      Assert.AreEqual("It's White's move now", move.ErrorMessage);
    }
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void DropNullPiece()
    {
      _board.GetDropMove(null, "5g");
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void DropPieceFromAnotherBoard()
    {
      var b = new Board();
      Shogi.InititBoard(b);
      _board.GetDropMove(b["1a"], "1e");
    }
    [TestMethod]
    public void TryDropTwoPawnInColumn()
    {
      Shogi.InititBoard(_board);
      var piece = _board["1g"];
      _board.White.Hand.Add(piece);
      _board["1g"] = null;
      var move = _board.GetDropMove(piece, "d1");
      Assert.AreEqual("Can't drop 歩 to the column 1 "+
        "because it already has one 歩", move.ErrorMessage);
    }
  }
}