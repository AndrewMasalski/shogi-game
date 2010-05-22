using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Primitives;
using Yasc.Utils;

namespace ShogiCore.UnitTests
{
  [TestClass]
  public class PositionTest
  {
    [TestMethod]
    public void Strings()
    {
      Assert.AreEqual(new Position(0, 0), Position.Parse("1a"));
      Assert.AreEqual(new Position(1, 0), Position.Parse("2a"));
      Assert.AreEqual(new Position(0, 1), Position.Parse("1b"));
    }
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void NullPositionString()
    {
      Position.Parse(null);
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void EmptyPositionString()
    {
      Position.Parse("");
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void TooShortPositionString()
    {
      Position.Parse("a");
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void TooLongPositionString()
    {
      Position.Parse("aaa");
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void OutOfRangePositionString1()
    {
      Position.Parse("1k");
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void OutOfRangePositionString2()
    {
      Position.Parse("0a");
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void OutOfRangePositionString3()
    {
      Position.Parse("1ù");
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void OutOfRangePositionString4()
    {
      Position.Parse("1+");
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void OutOfRangePosition1()
    {
      new Position(-1, 0);
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void OutOfRangePosition2()
    {
      new Position(0, 9);
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void OutOfRangePosition3()
    {
      new Position(9, 0);
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void OutOfRangePosition4()
    {
      new Position(0, -1);
    }
    [TestMethod]
    public void TestEquals()
    {
      Assert.IsFalse(Position.Parse("1i").Equals(new object()));
// ReSharper disable EqualExpressionComparison
      Assert.IsFalse(Position.Parse("1i") != Position.Parse("1i"));
// ReSharper restore EqualExpressionComparison
    }
    [TestMethod]
    public void CompareWithNull()
    {
      Assert.IsFalse(Position.Parse("1i").Equals(null));
    }
    [TestMethod]
    public void TestMath()
    {
      Assert.AreEqual(new Vector(), Position.Parse("1i") - Position.Parse("1i"));
    }
  }
}