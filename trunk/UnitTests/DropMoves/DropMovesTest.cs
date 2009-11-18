using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Utils;

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
    public void TryDropTwoPawnInColumn()
    {
      _board["1c"] = new Piece(_board.White, "歩");
      _board.White.Hand.Add(new Piece(_board.White, "歩"));
      var move = _board.GetDropMove("歩", "1d", _board.OneWhoMoves);
      Assert.AreEqual("Can't drop 歩 to the column 1 "+
                      "because it already has one 歩", move.ErrorMessage);
    }
    [TestMethod]
    public void DropTwoPawnInColumn()
    {
      _board["3c"] = new Piece(_board.White, "歩");
      _board.White.Hand.Add(new Piece(_board.White, "歩"));
      var move = _board.GetDropMove("歩", "1c", _board.OneWhoMoves);
      Assert.IsTrue(move.IsValid);
    }
   
    [TestMethod]
    public void TryDropPieceInBusyCell()
    {
      Shogi.InititBoard(_board);
      var piece = _board["1g"];
      _board.White.Hand.Add(piece);
      _board["1g"] = null;
      var move = _board.GetDropMove(piece.Type, "1c", _board.OneWhoMoves);
      Assert.AreEqual("Can drop piece to free cell only", move.ErrorMessage);
    }
    [TestMethod]
    public void TryDropPieceWhichIsOnTheBoard()
    {
      Shogi.InititBoard(_board);
      var move = _board.GetDropMove(_board["9a"].Type, "9e", _board.OneWhoMoves);
      Assert.AreEqual("Player doesn't have this piece in hand", move.ErrorMessage);
    }
    [TestMethod]
    public void TryDropPawnToTheLastLine()
    {
      Shogi.InititBoard(_board);
      var piece = _board["1c"];
      _board.White.Hand.Add(piece);
      _board["1i"] = null;
      var move = _board.GetDropMove(piece.Type, "1i", _board.OneWhoMoves);
      Assert.AreEqual("Can't drop 歩 to the last line", move.ErrorMessage);
    }
    [TestMethod]
    public void TryDropLanceToTheLastLine()
    {
      Shogi.InititBoard(_board);
      var piece = _board["1a"];
      _board.White.Hand.Add(piece);
      _board["1i"] = null;
      var move = _board.GetDropMove(piece.Type, "1i", _board.OneWhoMoves);
      Assert.AreEqual("Can't drop 香 to the last line", move.ErrorMessage);
    }
    [TestMethod]
    public void TryDropKnightToTheLastLines()
    {
      Shogi.InititBoard(_board);
      var piece = _board["2a"];
      _board.White.Hand.Add(piece);
      _board["2i"] = null;
      var move = _board.GetDropMove(piece.Type, "2i", _board.OneWhoMoves);
      Assert.AreEqual("Can't drop 桂 to the last two lines", move.ErrorMessage);

      _board["2h"] = null;
      move = _board.GetDropMove(piece.Type, "2h", _board.OneWhoMoves);
      Assert.AreEqual("Can't drop 桂 to the last two lines", move.ErrorMessage);
    }
    [TestMethod]
    public void TryDropPawnToMate()
    {
      //Black="K1i" White="R2h, G3g" WhiteHand="P"/>
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