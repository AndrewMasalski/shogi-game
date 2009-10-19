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
      vmth.Init("5c", new Piece(vmth.Board.White, "桂"));
      var positions = new HashSet<Position> {"4e", "6e"};
      vmth.TestMoves(positions);

      vmth.Board["4d"] = new Piece(vmth.Board.White, "歩");
      vmth.Board["5d"] = new Piece(vmth.Board.White, "歩");
      vmth.Board["6d"] = new Piece(vmth.Board.White, "歩");

      vmth.Board["4e"] = new Piece(vmth.Board.Black, "金");
      vmth.TestMoves(positions);

      vmth.Board["4e"] = new Piece(vmth.Board.White, "金");
      positions.ExceptWith(new Position[] { "4e" });
      vmth.TestMoves(positions);
    }

    [TestMethod]
    public void BlackOnG5()
    {
      var vmth = new ValidMovesTestHelper();
      vmth.Init("5g", new Piece(vmth.Board.Black, "桂"));
      var positions = new HashSet<Position> { "4e", "6e" };
      vmth.TestMoves(positions);

      vmth.Board["4f"] = new Piece(vmth.Board.Black, "歩");
      vmth.Board["5f"] = new Piece(vmth.Board.Black, "歩");
      vmth.Board["6f"] = new Piece(vmth.Board.Black, "歩");

      vmth.Board["4e"] = new Piece(vmth.Board.White, "金");
      vmth.TestMoves(positions);

      vmth.Board["4e"] = new Piece(vmth.Board.Black, "金");
      positions.ExceptWith(new Position[] { "4e" });
      vmth.TestMoves(positions);
    }
  }
}