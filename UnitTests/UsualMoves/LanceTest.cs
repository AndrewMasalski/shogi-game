using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Utils;

namespace UnitTests
{
  [TestClass]
  public class LanceTest
  {
    /* Имея те же свойства, что и ладья в шахматах, 
     * она может ходить на любое число полей по вертикали или горизонтали.
     * 
     * После переворота дополнительно к своим начальным возможностям получает 
     * право ходить на одно поле по любой диагонали.
     */

    [TestMethod]
    public void WhiteOnG5()
    {
      var vmth = new ValidMovesTestHelper();
      vmth.Init("g5", new Piece(vmth.Board.White, "香"));
      // Can't move to i5 without promotion
      var moves = new HashSet<Position> { "h5" };

      vmth.TestMoves(moves);

      vmth.Board["i5"] = new Piece(vmth.Board.Black, PieceType.桂);
      vmth.TestMoves(moves);

      vmth.Board["i5"] = new Piece(vmth.Board.White, PieceType.桂);
      moves.ExceptWith(new Position[] {  "i5" });
      vmth.TestMoves(moves);
    }

    
    [TestMethod]
    public void BlackOnG5()
    {
      var vmth = new ValidMovesTestHelper();
      vmth.Init("g5", new Piece(vmth.Board.Black, "香"));
      // Can't move to a5 without promotion
      var moves = new HashSet<Position> { "b5", "c5", "d5", "e5", "f5" };

      vmth.TestMoves(moves);

      vmth.Board["c5"] = new Piece(vmth.Board.White, PieceType.桂);
      moves.ExceptWith(new Position[] { "b5", });
      vmth.TestMoves(moves);

      vmth.Board["c5"] = new Piece(vmth.Board.Black, PieceType.桂);
      moves.ExceptWith(new Position[] { "c5" });
      vmth.TestMoves(moves);
    }
  }
}