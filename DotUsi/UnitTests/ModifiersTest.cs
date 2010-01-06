using System;
using DotUsi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
  [TestClass]
  public class ModifiersTest
  {
    #region ' DepthConstraint '

    [TestMethod]
    public void TestDepthConstraint()
    {
      Assert.AreEqual("depth 10", new DepthConstraint(10).Command);
    }

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void TestDepthConstraintLessThanZero()
    {
      new DepthConstraint(-1);
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void TestDepthConstraintEqualsZero()
    {
      new DepthConstraint(0);
    }

    #endregion

    #region ' NodesConstraint '

    [TestMethod]
    public void TestNodesConstraint()
    {
      Assert.AreEqual("nodes 10", new NodesConstraint(10).Command);
    }

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void TestNodesConstraintLessThanZero()
    {
      new NodesConstraint(-1);
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void TestNodesConstraintEqualsZero()
    {
      new NodesConstraint(0);
    }

    #endregion

    #region ' BlackIncrementModifier '

    [TestMethod]
    public void TestBlackIncrementModifier()
    {
      Assert.AreEqual("binc 1000", 
        new BlackIncrementModifier(TimeSpan.FromSeconds(1)).Command);
    }

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void TestBlackIncrementModifierLessThanZero()
    {
      new BlackIncrementModifier(TimeSpan.FromSeconds(-1));
    }

    #endregion
  }
}