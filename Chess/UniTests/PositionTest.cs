using System;
using Chess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace UniTests
{
  [TestClass]
  public class PositionTest
  {
    [TestMethod]
    public void Ctor()
    {
      var position = new Position();
      Assert.AreEqual(0, position.X);
      Assert.AreEqual(0, position.Y);

      position = new Position(3, 6);
      Assert.AreEqual(3, position.X);
      Assert.AreEqual(6, position.Y);

      MyAssert.ThrowsException<ArgumentOutOfRangeException>(() => new Position(-1, 0));
      MyAssert.ThrowsException<ArgumentOutOfRangeException>(() => new Position(1, -1));
      MyAssert.ThrowsException<ArgumentOutOfRangeException>(() => new Position(8, 1));
      MyAssert.ThrowsException<ArgumentOutOfRangeException>(() => new Position(1, 8));
    }
    [TestMethod]
    public void FileAndRank()
    {
      var position = new Position(3, 1);
      Assert.AreEqual("d", position.File);
      Assert.AreEqual(2, position.Rank);
      Assert.AreEqual("d2", position.ToString());
    }
    [TestMethod]
    public void Parse()
    {
      var position = Position.Parse("a1");
      Assert.AreEqual(0, position.X);
      Assert.AreEqual(0, position.Y);

      position = Position.Parse("h4");
      Assert.AreEqual(7, position.X);
      Assert.AreEqual(3, position.Y);

      position = Position.Parse("B7");
      Assert.AreEqual(1, position.X);
      Assert.AreEqual(6, position.Y);

      MyAssert.ThrowsException<ArgumentNullException>(() => Position.Parse(null));
      MyAssert.ThrowsException<ArgumentOutOfRangeException>(() => Position.Parse(""));
      MyAssert.ThrowsException<ArgumentOutOfRangeException>(() => Position.Parse("1"));
      MyAssert.ThrowsException<ArgumentOutOfRangeException>(() => Position.Parse("a11"));
      MyAssert.ThrowsException<ArgumentOutOfRangeException>(() => Position.Parse("!1"));
      MyAssert.ThrowsException<ArgumentOutOfRangeException>(() => Position.Parse("a0"));
      MyAssert.ThrowsException<FormatException>(() => Position.Parse("1a"));
      MyAssert.ThrowsException<FormatException>(() => Position.Parse("1a"));
      MyAssert.ThrowsException<ArgumentOutOfRangeException>(() => Position.Parse("a9"));
      MyAssert.ThrowsException<ArgumentOutOfRangeException>(() => Position.Parse("i8"));
    }
    [TestMethod]
    public void Equality()
    {
      Assert.IsFalse(Position.Parse("b2").Equals(Position.Parse("a1")));
      Assert.IsTrue(Position.Parse("b2").Equals((object)Position.Parse("b2")));
      Assert.IsFalse(Position.Parse("h1") == Position.Parse("a1"));
      Assert.IsTrue(Position.Parse("h1") != Position.Parse("a1"));
      Assert.IsFalse(Position.Parse("b2").Equals(new object()));
      Assert.IsFalse(Position.Parse("d3").Equals(null));
    }
    [TestMethod]
    public void HashCode()
    {
      Assert.AreEqual(
        new Position(1, 3).GetHashCode(),
        new Position(1, 3).GetHashCode());

      Assert.AreNotEqual(
        new Position(1, 3).GetHashCode(),
        new Position(3, 1).GetHashCode());
    }
    [TestMethod]
    public void VectorMath()
    {
      Assert.AreEqual(new Vector(3, 2), (Vector)Position.Parse("d3"));
      Assert.AreEqual(new Vector(), Position.Parse("d3") - Position.Parse("d3"));
      Assert.AreEqual(new Vector(2, 0), Position.Parse("d3") - new Vector(1, 2));
      Assert.AreEqual(new Vector(-2, 0), new Vector(1, 2) - Position.Parse("d3"));
      Assert.AreEqual(new Vector(4, 4), new Vector(1, 2) + Position.Parse("d3"));
      Assert.AreEqual(new Vector(4, 4), Position.Parse("d3") + new Vector(1, 2));
      Position position = new Vector(4, 4);
      Assert.AreEqual(Position.Parse("e5"), position);
    }
    [TestMethod]
    public void OnBoard()
    {
      Assert.AreEqual(64, Position.OnBoard.Count());
    }
  }
}