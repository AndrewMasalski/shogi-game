using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Utils;

namespace UnitTests.UsualMoves
{
  [TestClass]
  public class KingTest
  {
    [TestMethod]
    public void WhiteOnG5()
    {
      var vmth = new ValidMovesTestHelper();
      vmth.Init("5g", new Piece(vmth.Board.White, PieceType.玉));
      var moves = new HashSet<Position> {"4f", "5f", "6f", "4g", "6g", "4h", "5h", "6h",};
      vmth.TestMoves(moves);

      vmth.Board["5f"] = new Piece(vmth.Board.Black, PieceType.金);
      vmth.TestMoves(moves);

      vmth.Board["5f"] = new Piece(vmth.Board.White, PieceType.金);
      moves.Remove("5f");
      vmth.TestMoves(moves);
    }

    [TestMethod]
    public void BlackOnG5()
    {
      var vmth = new ValidMovesTestHelper();
      vmth.Init("5g", new Piece(vmth.Board.Black, PieceType.玉));
      var moves = new HashSet<Position> { "4f", "5f", "6f", "4g", "6g", "4h", "5h", "6h", };
      vmth.TestMoves(moves);

      vmth.Board["5f"] = new Piece(vmth.Board.White, PieceType.金);
      vmth.TestMoves(moves);

      vmth.Board["5f"] = new Piece(vmth.Board.Black, PieceType.金);
      moves.Remove("5f");
      vmth.TestMoves(moves);
    }
  }
}