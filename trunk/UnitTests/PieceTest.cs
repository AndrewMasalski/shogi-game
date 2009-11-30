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
    [TestMethod]
    public void CellTest()
    {
      var b = new Board();
      var c = b[0, 0];
      Assert.IsNull(c.Piece);
      c.Piece = null;
      Assert.IsNull(c.Piece);
      var piece = new Piece(b.White, "馬");
      c.Piece = piece;
      Assert.AreSame(c.Piece, piece);
      c.Piece = piece;
      Assert.AreSame(c.Piece, piece);
    }
  }
}