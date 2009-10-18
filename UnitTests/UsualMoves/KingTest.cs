using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Utils;

namespace UnitTests
{
  [TestClass]
  public class KingTest
  {
    [TestMethod]
    public void WhiteOnG5()
    {
      var vmth = new ValidMovesTestHelper();
      vmth.Init("g5", new Piece(vmth.Board.White, PieceType.玉));
      vmth.TestMoves(new HashSet<Position> {"f4", "f5", "f6", "g4", "g6", "h4", "h5", "h6",});

      vmth.Board["f5"] = new Piece(vmth.Board.Black, PieceType.金);
      vmth.TestMoves(new HashSet<Position> { "f4", "f5", "f6", "g4", "g6", "h4", "h5", "h6", });

      vmth.Board["f5"] = new Piece(vmth.Board.White, PieceType.金);
      vmth.TestMoves(new HashSet<Position> { "f4", "f6", "g4", "g6", "h4", "h5", "h6", });
    }

    [TestMethod]
    public void BlackOnG5()
    {
      var vmth = new ValidMovesTestHelper();
      vmth.Init("g5", new Piece(vmth.Board.Black, PieceType.玉));
      vmth.TestMoves(new HashSet<Position> { "f4", "f5", "f6", "g4", "g6", "h4", "h5", "h6", });

      vmth.Board["f5"] = new Piece(vmth.Board.White, PieceType.金);
      vmth.TestMoves(new HashSet<Position> { "f4", "f5", "f6", "g4", "g6", "h4", "h5", "h6", });

      vmth.Board["f5"] = new Piece(vmth.Board.Black, PieceType.金);
      vmth.TestMoves(new HashSet<Position> { "f4", "f6", "g4", "g6", "h4", "h5", "h6", });
    }
  }
}