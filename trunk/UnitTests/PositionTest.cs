using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore.Utils;

namespace UnitTests
{
  [TestClass]
  public class PositionTest
  {
    [TestMethod]
    public void Strings()
    {
      Assert.AreEqual(new Position(0, 0), new Position("1a"));
      Assert.AreEqual(new Position(1, 0), new Position("2a"));
      Assert.AreEqual(new Position(0, 1), new Position("1b"));
    }
  }
}