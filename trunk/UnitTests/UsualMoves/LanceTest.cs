using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Utils;

namespace UnitTests.UsualMoves
{
  [TestClass]
  public class LanceTest
  {
    [TestMethod]
    public void WhiteOnG5()
    {
      var vmth = new ValidMovesTestHelper();
      vmth.Init("5g", new Piece(vmth.Board.White, "香"));
      // Can't move to i5 without promotion
      var moves = new HashSet<Position> { "5h" };

      vmth.TestMoves(moves);

      vmth.Board["5i"] = new Piece(vmth.Board.Black, PieceType.桂);
      vmth.TestMoves(moves);

      vmth.Board["5i"] = new Piece(vmth.Board.White, PieceType.桂);
      moves.ExceptWith(new Position[] {  "5i" });
      vmth.TestMoves(moves);
    }
    [TestMethod]
    public void WhiteOnG5A()
    {
      var board = new Board();
      board["1a"] = new Piece(board.White, "香");
      Assert.IsTrue(board.GetUsualMove("1a", "1d", false).IsValid);
    }

    
    [TestMethod]
    public void BlackOnG5()
    {
      var vmth = new ValidMovesTestHelper();
      vmth.Init("5g", new Piece(vmth.Board.Black, "香"));
      // Can't move to a5 without promotion
      var moves = new HashSet<Position> {"5b", "5c", "5d", "5e", "5f"};

      vmth.TestMoves(moves);

      vmth.Board["5c"] = new Piece(vmth.Board.White, PieceType.桂);
      moves.ExceptWith(new Position[] { "5b", });
      vmth.TestMoves(moves);

      vmth.Board["5c"] = new Piece(vmth.Board.Black, PieceType.桂);
      moves.ExceptWith(new Position[] { "5c" });
      vmth.TestMoves(moves);
    }
  }
}