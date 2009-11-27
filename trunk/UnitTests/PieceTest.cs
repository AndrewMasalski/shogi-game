using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;

namespace UnitTests
{
  [TestClass]
  public class PieceTest
  {
    [TestMethod]
    public void TestPromote()
    {
      var b = new Board();
      var p = new Piece(b.White, "歩");
      p.IsPromoted = true;
      Assert.AreEqual("と", (string)p.Type);
      p.IsPromoted = false;
      Assert.AreEqual("歩", (string)p.Type);
    }
  }
}