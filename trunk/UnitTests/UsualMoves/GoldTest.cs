using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Utils;

namespace UnitTests
{
  [TestClass]
  public class GoldTest
  {
    [TestMethod]
    public void WhiteOnG5()
    {
      WhiteOnG5("金");
      WhiteOnG5(PieceType.銀.Promote());
      WhiteOnG5(PieceType.桂.Promote());
      WhiteOnG5(PieceType.歩.Promote());
    }

    private static void WhiteOnG5(PieceType type)
    {
      var vmth = new ValidMovesTestHelper();
      vmth.Init("g5", new Piece(vmth.Board.White, type));
      var positions = new HashSet<Position> {"h4", "h5", "h6", "g4", "g6", "f5"};
      vmth.TestMoves(positions);

      vmth.Board["f5"] = new Piece(vmth.Board.Black, "金");
      vmth.TestMoves(positions);

      vmth.Board["f5"] = new Piece(vmth.Board.White, "金");
      positions.ExceptWith(new Position[] { "f5" });
      vmth.TestMoves(positions);
    }

    [TestMethod]
    public void BlackOnG5()
    {
      BlackOnG5("金");
      BlackOnG5(PieceType.銀.Promote());
      BlackOnG5(PieceType.桂.Promote());
      BlackOnG5(PieceType.歩.Promote());
    }

    private static void BlackOnG5(PieceType type)
    {
      var vmth = new ValidMovesTestHelper();
      vmth.Init("g5", new Piece(vmth.Board.Black, type));
      var positions = new HashSet<Position> { "f4", "f5", "f6", "g4", "g6", "h5" };
      vmth.TestMoves(positions);

      vmth.Board["f5"] = new Piece(vmth.Board.White, "金");
      vmth.TestMoves(positions);

      vmth.Board["f5"] = new Piece(vmth.Board.Black, "金");
      positions.ExceptWith(new Position[] { "f5" });
      vmth.TestMoves(positions);
    }
  }
}