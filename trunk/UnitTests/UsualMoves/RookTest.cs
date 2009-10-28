using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Utils;

namespace UnitTests.UsualMoves
{
  [TestClass]
  public class RookTest
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
      vmth.Init("5g", new Piece(vmth.Board.White, "飛"));
      var moves = new HashSet<Position> {
                                          "1g", "2g", "3g", "4g", "6g", "7g", "8g", "9g",  
                                          "5a", "5b", "5c", "5d", "5e", "5f","5h", "5i",  
                                        };

      vmth.TestMoves(moves);

      vmth.Board["5f"] = new Piece(vmth.Board.Black, PieceType.桂);
      moves.ExceptWith(new Position[] { "5a", "5b", "5c", "5d", "5e" });
      vmth.TestMoves(moves);

      vmth.Board["5f"] = new Piece(vmth.Board.White, PieceType.桂);
      moves.ExceptWith(new Position[] {  "5f" });
      vmth.TestMoves(moves);
    }

    
    [TestMethod]
    public void BlackOnG5()
    {
      var vmth = new ValidMovesTestHelper();
      vmth.Init("5g", new Piece(vmth.Board.Black, "飛"));
      var moves = new HashSet<Position> {
                                          "1g", "2g", "3g", "4g", "6g", "7g", "8g", "9g",  
                                          "5a", "5b", "5c", "5d", "5e", "5f","5h", "5i",  
                                        };

      vmth.TestMoves(moves);

      vmth.Board["5f"] = new Piece(vmth.Board.White, PieceType.桂);
      moves.ExceptWith(new Position[] { "5a", "5b", "5c", "5d", "5e" });
      vmth.TestMoves(moves);

      vmth.Board["5f"] = new Piece(vmth.Board.Black, PieceType.桂);
      moves.ExceptWith(new Position[] { "5f" });
      vmth.TestMoves(moves);
    }
  }
}