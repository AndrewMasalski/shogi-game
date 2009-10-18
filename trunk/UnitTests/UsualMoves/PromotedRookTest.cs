﻿using System.Collections.Generic;
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
      vmth.Init("g5", new Piece(vmth.Board.White, "竜"));
      var initialValidMoves = new HashSet<Position> {
                                                      "g1", "g2", "g3", "g7", "g8", "g9",  
                                                      "a5", "b5", "c5", "d5", "e5", "i5",  
                                                      "f4", "f5", "f6", 
                                                      "g4", "g6",  
                                                      "h4", "h5", "h6", };

      vmth.TestMoves(initialValidMoves);

      vmth.Board["f5"] = new Piece(vmth.Board.Black, PieceType.桂);
      initialValidMoves.ExceptWith(new Position[] { "a5", "b5", "c5", "d5", "e5" });
      vmth.TestMoves(initialValidMoves);

      vmth.Board["f5"] = new Piece(vmth.Board.White, PieceType.桂);
      initialValidMoves.ExceptWith(new Position[] {  "f5" });
      vmth.TestMoves(initialValidMoves);
    }

    
    [TestMethod]
    public void BlackOnG5()
    {
      var vmth = new ValidMovesTestHelper();
      vmth.Init("g5", new Piece(vmth.Board.Black, "竜"));
      var initialValidMoves = new HashSet<Position> {
                                                      "g1", "g2", "g3", "g7", "g8", "g9",  
                                                      "a5", "b5", "c5", "d5", "e5", "i5",  
                                                      "f4", "f5", "f6", 
                                                      "g4", "g6",  
                                                      "h4", "h5", "h6", };

      vmth.TestMoves(initialValidMoves);

      vmth.Board["f5"] = new Piece(vmth.Board.White, PieceType.桂);
      initialValidMoves.ExceptWith(new Position[] { "a5", "b5", "c5", "d5", "e5"});
      vmth.TestMoves(initialValidMoves);

      vmth.Board["f5"] = new Piece(vmth.Board.Black, PieceType.桂);
      initialValidMoves.ExceptWith(new Position[] {  "f5" });
      vmth.TestMoves(initialValidMoves);
    }
  }
}