using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Utils;

namespace UnitTests
{
  [TestClass]
  public class PromotedRookTest
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
      vmth.Init("5g", new Piece(vmth.Board.White, "竜"));
      var initialValidMoves = new HashSet<Position> {
            "1g", "2g", "3g", "7g", "8g", "9g",  
            "5a", "5b", "5c", "5d", "5e", "5i",  
            "4f", "5f", "6f", 
            "4g", "6g",  
            "4h", "5h", "6h", };

      vmth.TestMoves(initialValidMoves);

      vmth.Board["5f"] = new Piece(vmth.Board.Black, PieceType.桂);
      initialValidMoves.ExceptWith(new Position[] {"5a", "5b", "5c", "5d", "5e"});
      vmth.TestMoves(initialValidMoves);

      vmth.Board["5f"] = new Piece(vmth.Board.White, PieceType.桂);
      initialValidMoves.ExceptWith(new Position[] {  "5f" });
      vmth.TestMoves(initialValidMoves);
    }

    
    [TestMethod]
    public void BlackOnG5()
    {
      var vmth = new ValidMovesTestHelper();
      vmth.Init("5g", new Piece(vmth.Board.Black, "竜"));
      var initialValidMoves = new HashSet<Position> {
        "1g", "2g", "3g", "7g", "8g", "9g",  
        "5a", "5b", "5c", "5d", "5e", "5i",  
        "4f", "5f", "6f", 
        "4g", "6g",
        "4h", "5h", "6h", 
      };

      vmth.TestMoves(initialValidMoves);

      vmth.Board["5f"] = new Piece(vmth.Board.White, PieceType.桂);
      initialValidMoves.ExceptWith(new Position[] { "5a","5b", "5c", "5d","5e"});
      vmth.TestMoves(initialValidMoves);

      vmth.Board["5f"] = new Piece(vmth.Board.Black, PieceType.桂);
      initialValidMoves.ExceptWith(new Position[] {  "5f" });
      vmth.TestMoves(initialValidMoves);
    }
  }
}