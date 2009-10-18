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
      vmth.Init("g5", new Piece(vmth.Board.White, "銀"));
      var positions = new HashSet<Position> {"h4", "h5", "h6", "f4", "f6"};
      vmth.TestMoves(positions);

      vmth.Board["h5"] = new Piece(vmth.Board.Black, "金");
      vmth.TestMoves(positions);

      vmth.Board["h5"] = new Piece(vmth.Board.White, "金");
      positions.ExceptWith(new Position[] { "h5" });
      vmth.TestMoves(positions);
    }

    [TestMethod]
    public void BlackOnG5()
    {
      var vmth = new ValidMovesTestHelper();
      vmth.Init("g5", new Piece(vmth.Board.Black, "銀"));
      var positions = new HashSet<Position> { "f4", "f5", "f6", "h4", "h6" };
      vmth.TestMoves(positions);

      vmth.Board["f5"] = new Piece(vmth.Board.White, "金");
      vmth.TestMoves(positions);

      vmth.Board["f5"] = new Piece(vmth.Board.Black, "金");
      positions.ExceptWith(new Position[] { "f5" });
      vmth.TestMoves(positions);
    }
  }
}