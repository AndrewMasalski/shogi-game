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
      Assert.AreEqual(new Position(0, 0), new Position("1a"));
      Assert.AreEqual(new Position(1, 0), new Position("2a"));
      Assert.AreEqual(new Position(0, 1), new Position("1b"));
    }
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void NullPositionString()
    {
      new Position(null);
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void EmptyPositionString()
    {
      new Position("");
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void TooShortPositionString()
    {
      new Position("a");
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void TooLongPositionString()
    {
      new Position("aaa");
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void OutOfRangePositionString1()
    {
      new Position("1k");
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void OutOfRangePositionString2()
    {
      new Position("0a");
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void OutOfRangePositionString3()
    {
      new Position("1ù");
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void OutOfRangePositionString4()
    {
      new Position("1+");
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
      Assert.IsFalse(new Position("1i").Equals(new object()));
// ReSharper disable EqualExpressionComparison
      Assert.IsFalse(new Position("1i") != new Position("1i"));
// ReSharper restore EqualExpressionComparison
    }
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void CompareWithNull()
    {
      Assert.IsFalse(new Position("1i").Equals(null));
    }
    [TestMethod]
    public void TestMath()
    {
      Assert.AreEqual(new Vector(), new Position("1i") - new Position("1i"));
    }
  }
}