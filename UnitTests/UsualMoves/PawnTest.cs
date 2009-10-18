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
      vmth.Init("g5", new Piece(vmth.Board.White, PieceType.歩));
      vmth.TestMoves(new HashSet<Position>{ "h5", });

      vmth.Board["h5"] = new Piece(vmth.Board.Black, PieceType.桂);
      vmth.TestMoves(new HashSet<Position> { "h5", });

      vmth.Board["h5"] = new Piece(vmth.Board.White, PieceType.桂);
      vmth.TestMoves(new HashSet<Position>());
    }

    [TestMethod]
    public void BlackOnG5()
    {
      var vmth = new ValidMovesTestHelper();
      vmth.Init("g5", new Piece(vmth.Board.Black, PieceType.歩));
      vmth.TestMoves(new HashSet<Position> { "f5", });

      vmth.Board["f5"] = new Piece(vmth.Board.White, PieceType.桂);
      vmth.TestMoves(new HashSet<Position> { "f5", });

      vmth.Board["f5"] = new Piece(vmth.Board.Black, PieceType.桂);
      vmth.TestMoves(new HashSet<Position>());
    }
  }
}