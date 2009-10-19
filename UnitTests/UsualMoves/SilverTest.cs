using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Utils;

namespace UnitTests
{
  [TestClass]
  public class SilverTest
  {
    [TestMethod]
    public void WhiteOnG5()
    {
      var vmth = new ValidMovesTestHelper();
      vmth.Init("5g", new Piece(vmth.Board.White, "銀"));
      var positions = new HashSet<Position> { "4h", "5h", "6h", "4f", "6f" };
      vmth.TestMoves(positions);

      vmth.Board["5h"] = new Piece(vmth.Board.Black, "金");
      vmth.TestMoves(positions);

      vmth.Board["5h"] = new Piece(vmth.Board.White, "金");
      positions.ExceptWith(new Position[] { "5h" });
      vmth.TestMoves(positions);
    }

    [TestMethod]
    public void BlackOnG5()
    {
      var vmth = new ValidMovesTestHelper();
      vmth.Init("5g", new Piece(vmth.Board.Black, "銀"));
      var positions = new HashSet<Position> { "4f", "5f", "6f", "4h", "6h" };
      vmth.TestMoves(positions);

      vmth.Board["5f"] = new Piece(vmth.Board.White, "金");
      vmth.TestMoves(positions);

      vmth.Board["5f"] = new Piece(vmth.Board.Black, "金");
      positions.ExceptWith(new Position[] { "5f" });
      vmth.TestMoves(positions);
    }
  }
}