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
      var clone = new BoardSnapshot(original,
        new UsualMoveSnapshot("3c", "3d", false));
      
      Assert.IsNull(clone["3c"]);
      Assert.IsNotNull(clone["3d"]);

      Assert.IsNotNull(original["3c"]);
      Assert.IsNull(original["3d"]);
    }
  }
}