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