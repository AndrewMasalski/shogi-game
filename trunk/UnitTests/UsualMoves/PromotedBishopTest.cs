using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Utils;

namespace UnitTests.UsualMoves
{
  [TestClass]
  public class PromotedBishopTest
  {
    [TestMethod]
    public void WhiteOnG5()
    {
      var vmth = new ValidMovesTestHelper();
      vmth.Init("5g", new Piece(vmth.Board.White, "馬"));
      var initialValidMoves = new HashSet<Position> {
                                                      "1c", "2d", "3e", "4f", "6h", "7i",
                                                      "9c", "8d", "7e", "6f", "4h", "3i",
                                                      "4g", "6g", "5f", "5h",
                                                    };

      vmth.TestMoves(initialValidMoves);

      vmth.Board["7e"] = new Piece(vmth.Board.Black, PieceType.桂);
      initialValidMoves.ExceptWith(new Position[] { "9c", "8d" });
      vmth.TestMoves(initialValidMoves);

      vmth.Board["7e"] = new Piece(vmth.Board.White, PieceType.桂);
      initialValidMoves.ExceptWith(new Position[] { "7e" });
      vmth.TestMoves(initialValidMoves);
    }

    
    [TestMethod]
    public void BlackOnG5()
    {
      var vmth = new ValidMovesTestHelper();
      vmth.Init("5g", new Piece(vmth.Board.Black, "馬"));
      var initialValidMoves = new HashSet<Position> {
                                                      "1c", "2d", "3e", "4f", "6h", "7i",
                                                      "9c", "8d", "7e", "6f", "4h", "3i",
                                                      "4g", "6g", "5f", "5h",
                                                    };

      vmth.TestMoves(initialValidMoves);

      vmth.Board["7e"] = new Piece(vmth.Board.White, PieceType.桂);
      initialValidMoves.ExceptWith(new Position[] { "9c", "8d" });
      vmth.TestMoves(initialValidMoves);

      vmth.Board["7e"] = new Piece(vmth.Board.Black, PieceType.桂);
      initialValidMoves.ExceptWith(new Position[] { "7e" });
      vmth.TestMoves(initialValidMoves);

    }
  }
}