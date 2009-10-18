using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore.Utils;

namespace UnitTests
{
  [TestClass]
  public class CellTest
  {
    [TestMethod]
    public void Strings()
    {
      Assert.AreEqual(0, new Position("a1").X);
      Assert.AreEqual(0, new Position("b1").X);

      Assert.AreEqual(0, new Position("a1").Y);
      Assert.AreEqual(0, new Position("a2").Y);

      Assert.AreEqual(0, new Position("a1").Y);
      Assert.AreEqual(1, new Position("b1").Y);

      Assert.AreEqual(0, new Position("a1").X);
      Assert.AreEqual(1, new Position("a2").X);
    }
  }
}