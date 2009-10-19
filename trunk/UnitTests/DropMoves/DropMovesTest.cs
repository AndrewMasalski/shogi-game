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
      var piece = _board["a9"];
      _board.Black.Hand.Add(piece);
      _board["a9"] = null;
      _board.OneWhoMoves = _board.Black;
      var move = _board.GetDropMove(piece, "e9");
      _board.MakeMove(move);
      Assert.AreSame(piece, _board["e9"]);
      Assert.AreSame(_board.Black, piece.Owner);
    }
    [TestMethod]
    public void MovesOrderMaintenanceFalse()
    {
      Shogi.InititBoard(_board);
      var piece = _board["a9"];
      _board.Black.Hand.Add(piece);
      _board["a9"] = null;
      _board.IsMovesOrderMaintained = false;
      var move = _board.GetDropMove(piece, "e9");
      _board.MakeMove(move);
      Assert.AreSame(piece, _board["e9"]);
      Assert.AreSame(_board.Black, piece.Owner);
    }
    [TestMethod]
    public void MovesOrderMaintenanceTrue()
    {
      Shogi.InititBoard(_board);
      var piece = _board["a9"];
      _board.Black.Hand.Add(piece);
      _board["a9"] = null;
      _board.IsMovesOrderMaintained = true;
      var move = _board.GetDropMove(piece, "e9");
      Assert.AreEqual("It's White's move now", move.ErrorMessage);
    }
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void DropNullPiece()
    {
      _board.GetDropMove(null, "g5");
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void DropPieceFromAnotherBoard()
    {
      var b = new Board();
      Shogi.InititBoard(b);
      _board.GetDropMove(b["a1"], "e1");
    }
  }
}