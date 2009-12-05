using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;

namespace UnitTests.DropMoves
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
      var move = _board.GetDropMove(piece.Type, "9e", _board.Black);
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
      var move = _board.GetDropMove(piece.Type, "9e", _board.OneWhoMoves);
      Assert.AreEqual("Player doesn't have this piece in hand", move.ErrorMessage);
    }
    [TestMethod]
    public void MovesOrderMaintenanceTrue()
    {
      var blackPiece = new Piece(_board.Black, "歩");
      _board.Black.Hand.Add(blackPiece);
      var whitePiece = new Piece(_board.White, "歩");
      _board.White.Hand.Add(whitePiece);
      _board.OneWhoMoves = _board.White;
      var move = _board.GetDropMove(blackPiece, "5e");
      Assert.AreEqual("It's White's move now", move.ErrorMessage);
    }
  
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void DropNullPiece()
    {
      _board.GetDropMove(null, "5g");
    }

    [TestMethod]
    public void TryDropPieceWhichIsOnTheBoard()
    {
      Shogi.InititBoard(_board);
      var move = _board.GetDropMove(_board["9a"].Type, "9e", _board.OneWhoMoves);
      Assert.AreEqual("Player doesn't have this piece in hand", move.ErrorMessage);
    }
  }
}