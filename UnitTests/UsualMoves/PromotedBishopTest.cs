using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Utils;

namespace UnitTests
{
  [TestClass]
  public class PromotedBishopTest
  {
    [TestMethod]
    public void WhiteOnG5()
    {
      var vmth = new ValidMovesTestHelper();
      vmth.Init("g5", new Piece(vmth.Board.White, "馬"));
      var initialValidMoves = new HashSet<Position> {
          "c1", "d2", "e3", "f4", "h6", "i7",
          "c9", "d8", "e7", "f6", "h4", "i3",
          "g4", "g6", "f5", "h5",
        };

      vmth.TestMoves(initialValidMoves);

      vmth.Board["e7"] = new Piece(vmth.Board.Black, PieceType.桂);
      initialValidMoves.ExceptWith(new Position[] { "c9", "d8" });
      vmth.TestMoves(initialValidMoves);

      vmth.Board["e7"] = new Piece(vmth.Board.White, PieceType.桂);
      initialValidMoves.ExceptWith(new Position[] { "e7" });
      vmth.TestMoves(initialValidMoves);
    }

    
    [TestMethod]
    public void BlackOnG5()
    {
      var vmth = new ValidMovesTestHelper();
      vmth.Init("g5", new Piece(vmth.Board.Black, "馬"));
      var initialValidMoves = new HashSet<Position> {
            "c1", "d2", "e3", "f4", "h6", "i7",
            "c9", "d8", "e7", "f6", "h4", "i3",
            "g4", "g6", "f5", "h5",
          };

      vmth.TestMoves(initialValidMoves);

      vmth.Board["e7"] = new Piece(vmth.Board.White, PieceType.桂);
      initialValidMoves.ExceptWith(new Position[] { "c9", "d8" });
      vmth.TestMoves(initialValidMoves);

      vmth.Board["e7"] = new Piece(vmth.Board.Black, PieceType.桂);
      initialValidMoves.ExceptWith(new Position[] { "e7" });
      vmth.TestMoves(initialValidMoves);

    }
  }
}