using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Moves;
using Yasc.ShogiCore.Utils;

namespace UnitTests
{
  [TestClass]
  public class BoardSnapshotTest
  {
    [TestMethod]
    public void Immutability()
    {
      var board = new Board();
      Shogi.InititBoard(board);
      var original = new BoardSnapshot(board);
      var clone = new BoardSnapshot(original, new UsualMoveSnapshot("c3", "d3", false));
      
      Assert.IsNull(clone["c3"]);
      Assert.IsNotNull(clone["d3"]);

      Assert.IsNotNull(original["c3"]);
      Assert.IsNull(original["d3"]);
    }
  }
}