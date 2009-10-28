using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Moves;
using Yasc.ShogiCore.Utils;

namespace UnitTests
{
  [TestClass]
  public class PawnTest
  {
    [TestMethod]
    public void WhiteOnG5()
    {
      var vmth = new ValidMovesTestHelper();
      vmth.Init("5g", new Piece(vmth.Board.White, PieceType.歩));
      vmth.TestMoves(new HashSet<Position>{ "5h", });

      vmth.Board["5h"] = new Piece(vmth.Board.Black, PieceType.桂);
      vmth.TestMoves(new HashSet<Position> { "5h", });

      vmth.Board["5h"] = new Piece(vmth.Board.White, PieceType.桂);
      vmth.TestMoves(new HashSet<Position>());
    }

    [TestMethod]
    public void BlackOnG5()
    {
      var vmth = new ValidMovesTestHelper();
      vmth.Init("5g", new Piece(vmth.Board.Black, PieceType.歩));
      vmth.TestMoves(new HashSet<Position> { "5f", });

      vmth.Board["5f"] = new Piece(vmth.Board.White, PieceType.桂);
      vmth.TestMoves(new HashSet<Position> { "5f", });

      vmth.Board["5f"] = new Piece(vmth.Board.Black, PieceType.桂);
      vmth.TestMoves(new HashSet<Position>());
    }

    [TestMethod]
    public void TestPromotionRow()
    {
      var b = new Board();
      b["1e"] = new Piece(b.White, "歩");
      Assert.AreEqual("Can't promote 歩 when move from line e to line f",
        b.GetMove("1e-1f+").ErrorMessage);
    }
  }
}