using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Utils;

namespace UnitTests
{
  [TestClass]
  public class KnightTest
  {
    [TestMethod]
    public void WhiteOnG5()
    {
      var vmth = new ValidMovesTestHelper();
      vmth.Init("c5", new Piece(vmth.Board.White, "桂"));
      var positions = new HashSet<Position> {"e4", "e6"};
      vmth.TestMoves(positions);

      vmth.Board["d4"] = new Piece(vmth.Board.White, "歩");
      vmth.Board["d5"] = new Piece(vmth.Board.White, "歩");
      vmth.Board["d6"] = new Piece(vmth.Board.White, "歩");

      vmth.Board["e4"] = new Piece(vmth.Board.Black, "金");
      vmth.TestMoves(positions);

      vmth.Board["e4"] = new Piece(vmth.Board.White, "金");
      positions.ExceptWith(new Position[] { "e4" });
      vmth.TestMoves(positions);
    }

    [TestMethod]
    public void BlackOnG5()
    {
      var vmth = new ValidMovesTestHelper();
      vmth.Init("g5", new Piece(vmth.Board.Black, "桂"));
      var positions = new HashSet<Position> { "e4", "e6" };
      vmth.TestMoves(positions);

      vmth.Board["f4"] = new Piece(vmth.Board.Black, "歩");
      vmth.Board["f5"] = new Piece(vmth.Board.Black, "歩");
      vmth.Board["f6"] = new Piece(vmth.Board.Black, "歩");

      vmth.Board["e4"] = new Piece(vmth.Board.White, "金");
      vmth.TestMoves(positions);

      vmth.Board["e4"] = new Piece(vmth.Board.Black, "金");
      positions.ExceptWith(new Position[] { "e4" });
      vmth.TestMoves(positions);
    }
  }
}