using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Utils;

namespace UnitTests.UsualMoves
{
  [TestClass]
  public class BishopTest
  {
    [TestMethod]
    public void WhiteOnG5()
    {
      var vmth = new ValidMovesTestHelper();
      vmth.Init("5g", new Piece(vmth.Board.White, "角"));
      var moves = new HashSet<Position> {
                                          "1c", "2d", "3e", "4f", "6h", "7i",
                                          "9c", "8d", "7e", "6f", "4h", "3i",
                                        };

      vmth.TestMoves(moves);

      vmth.Board["7e"] = new Piece(vmth.Board.Black, PieceType.桂);
      moves.ExceptWith(new Position[] { "9c", "8d" });
      vmth.TestMoves(moves);

      vmth.Board["7e"] = new Piece(vmth.Board.White, PieceType.桂);
      moves.ExceptWith(new Position[] { "7e" });
      vmth.TestMoves(moves);
    }

    [TestMethod]
    public void BlackOnG5()
    {
      var vmth = new ValidMovesTestHelper();
      vmth.Init("5g", new Piece(vmth.Board.Black, "角"));
      var moves = new HashSet<Position> {
                                          "1c", "2d", "3e", "4f", "6h", "7i",
                                          "9c", "8d", "7e", "6f", "4h", "3i",
                                        };

      vmth.TestMoves(moves);

      vmth.Board["7e"] = new Piece(vmth.Board.White, PieceType.桂);
      moves.ExceptWith(new Position[] { "9c", "8d" });
      vmth.TestMoves(moves);

      vmth.Board["7e"] = new Piece(vmth.Board.Black, PieceType.桂);
      moves.ExceptWith(new Position[] { "7e" });
      vmth.TestMoves(moves);
    }
    [TestMethod]
    public void BlackOnA1()
    {
      var vmth = new ValidMovesTestHelper();
      vmth.Init("1a", new Piece(vmth.Board.Black, "角"));
      vmth.TestMoves(new HashSet<Position> {
                                             "2b", "3c","4d", "5e", "6f", "7g","8h","9i"
                                           });
    }
  }
}