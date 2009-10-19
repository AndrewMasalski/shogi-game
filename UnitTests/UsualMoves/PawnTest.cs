using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Utils;

namespace UnitTests
{
  [TestClass]
  public class PawnTest
  {
    [TestMethod]
    public void WhiteOnG5()
    {
      var vmth = new ValidMovesTestHelper();
      vmth.Init("5g", new Piece(vmth.Board.White, PieceType.歩));
      vmth.TestMoves(new HashSet<Position>{ "5h", });

      vmth.Board["5h"] = new Piece(vmth.Board.Black, PieceType.桂);
      vmth.TestMoves(new HashSet<Position> { "5h", });

      vmth.Board["5h"] = new Piece(vmth.Board.White, PieceType.桂);
      vmth.TestMoves(new HashSet<Position>());
    }

    [TestMethod]
    public void BlackOnG5()
    {
      var vmth = new ValidMovesTestHelper();
      vmth.Init("5g", new Piece(vmth.Board.Black, PieceType.歩));
      vmth.TestMoves(new HashSet<Position> { "5f", });

      vmth.Board["5f"] = new Piece(vmth.Board.White, PieceType.桂);
      vmth.TestMoves(new HashSet<Position> { "5f", });

      vmth.Board["5f"] = new Piece(vmth.Board.Black, PieceType.桂);
      vmth.TestMoves(new HashSet<Position>());
    }
  }
}