using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Moves;

namespace UnitTests
{
  [TestClass, NUnit.Framework.TestFixture]
  public class MoveTest
  {
    [TestMethod, NUnit.Framework.Test]
    public void TestMoveToSting()
    {
      var b = new Board();
      var move = b.GetMove("3c-3d");
      var m = (UsualMove) move;
      Assert.AreEqual(new Position(2, 2), m.From);
      Assert.AreEqual(new Position(2, 3), m.To);
      Assert.IsFalse(m.IsPromoting);
      Assert.AreEqual("3c-3d", move.ToString());
    }
    [TestMethod, NUnit.Framework.Test]
    public void TestUsualMoveToString()
    {
      var b = new Board();
      var move = b.GetMove("3c-3d+");
      var m = (UsualMove)move;
      Assert.AreEqual(new Position(2, 2), m.From);
      Assert.AreEqual(new Position(2, 3), m.To);
      Assert.IsTrue(m.IsPromoting);
      Assert.AreEqual("3c-3d+", move.ToString());
    }
    [TestMethod, NUnit.Framework.Test]
    public void TestUsualMoveNoPropmotionToString()
    {
      var b = new Board();
      var move = b.GetMove("3c-3d=");
      var m = (UsualMove)move;
      Assert.AreEqual(new Position(2, 2), m.From);
      Assert.AreEqual(new Position(2, 3), m.To);
      Assert.IsFalse(m.IsPromoting);
      Assert.AreEqual("3c-3d", move.ToString());
    }
    [TestMethod, NUnit.Framework.Test]
    public void TestDropMoveToString()
    {
      var b = new Board();
      var move = b.GetMove("P'3d");
      var m = (DropMove)move;
      Assert.AreEqual("歩", (string)m.PieceType);
      Assert.AreEqual(new Position(2, 3), m.To);
      Assert.AreEqual("P'3d", move.ToString());
    }
  }
}