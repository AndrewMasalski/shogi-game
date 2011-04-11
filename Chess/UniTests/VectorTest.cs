using Chess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UniTests
{
  [TestClass]
  public class VectorTest
  {
    [TestMethod]
    public void Ctor()
    {
      var vector = new Vector();
      Assert.AreEqual(0, vector.X);
      Assert.AreEqual(0, vector.Y);

      vector = new Vector(5, 6);
      Assert.AreEqual(5, vector.X);
      Assert.AreEqual(6, vector.Y);
    }

    [TestMethod]
    public void MultiplyOperator()
    {
      var vector = new Vector(2, 5)*new Vector(3, 7);
      Assert.AreEqual(6, vector.X);
      Assert.AreEqual(35, vector.Y);
    }

    [TestMethod]
    public void AddOperator()
    {
      var vector = new Vector(2, 5) + new Vector(3, 7);
      Assert.AreEqual(5, vector.X);
      Assert.AreEqual(12, vector.Y);
    }

    [TestMethod]
    public void Equality()
    {
      Assert.IsTrue(new Vector(1, 3).Equals(new Vector(1, 3)));
      Assert.IsTrue(new Vector(1, 3).Equals((object)new Vector(1, 3)));
      Assert.IsFalse(new Vector(1, 3).Equals(null));
      Assert.IsFalse(new Vector(1, 3).Equals(new object()));
      Assert.IsFalse(new Vector(1, 3) == new Vector(3, 1));
      Assert.IsTrue(new Vector(1, 3) != new Vector(3, 1));
    }

    [TestMethod]
    public void HashCode()
    {
      Assert.AreEqual(
        new Vector(1, 3).GetHashCode(), 
        new Vector(1, 3).GetHashCode());
      
      Assert.AreNotEqual(
        new Vector(1, 3).GetHashCode(), 
        new Vector(3, 1).GetHashCode());
    }

    [TestMethod]
    public void String()
    {
      Assert.AreEqual("(13,12)", new Vector(13, 12).ToString());
    }
  }
}
