using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Utils;

namespace UnitTests.UsualMoves
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
      vmth.Init("5g", new Piece(vmth.Board.White, type));
      var positions = new HashSet<Position> {"4h", "5h", "6h", "4g", "6g", "5f"};
      vmth.TestMoves(positions);

      vmth.Board["5f"] = new Piece(vmth.Board.Black, "金");
      vmth.TestMoves(positions);

      vmth.Board["5f"] = new Piece(vmth.Board.White, "金");
      positions.ExceptWith(new Position[] { "5f" });
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
      vmth.Init("5g", new Piece(vmth.Board.Black, type));
      var positions = new HashSet<Position> {"4f", "5f", "6f", "4g", "6g", "5h"};
      vmth.TestMoves(positions);

      vmth.Board["5f"] = new Piece(vmth.Board.White, "金");
      vmth.TestMoves(positions);

      vmth.Board["5f"] = new Piece(vmth.Board.Black, "金");
      positions.ExceptWith(new Position[] { "5f" });
      vmth.TestMoves(positions);
    }
  }
}