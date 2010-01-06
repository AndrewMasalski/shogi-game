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

    #region ' BlackTimeModifier '

    [TestMethod]
    public void TestBlackTimeModifier()
    {
      Assert.AreEqual("btime 1000", 
        new BlackTimeModifier(TimeSpan.FromSeconds(1)).Command);
    }

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void TestBlackTimeModifierLessThanZero()
    {
      new BlackTimeModifier(TimeSpan.FromSeconds(-1));
    }

    #endregion

    #region ' WhiteIncrementModifier '

    [TestMethod]
    public void TestWhiteIncrementModifier()
    {
      Assert.AreEqual("winc 1000", 
        new WhiteIncrementModifier(TimeSpan.FromSeconds(1)).Command);
    }

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void TestWhiteIncrementModifierLessThanZero()
    {
      new WhiteIncrementModifier(TimeSpan.FromSeconds(-1));
    }

    #endregion

    #region ' WhiteTimeModifier '

    [TestMethod]
    public void TestWhiteTimeModifier()
    {
      Assert.AreEqual("wtime 1000", 
        new WhiteTimeModifier(TimeSpan.FromSeconds(1)).Command);
    }

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void TestWhiteTimeModifierLessThanZero()
    {
      new WhiteTimeModifier(TimeSpan.FromSeconds(-1));
    }

    #endregion

    #region ' ByoyomiModifie '

    [TestMethod]
    public void TestByoyomiModifier()
    {
      Assert.AreEqual("byoyomi 1000", 
        new ByoyomiModifier(TimeSpan.FromSeconds(1)).Command);
    }

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void TestByoyomiModifierLessThanZero()
    {
      new ByoyomiModifier(TimeSpan.FromSeconds(-1));
    }

    #endregion

    [TestMethod]
    public void InfiniteModifierTest()
    {
      Assert.AreEqual("infinite", new InfiniteModifier().Command);
    }

    #region ' MoveTimeModifier '

    [TestMethod]
    public void TestMoveTimeModifier()
    {
      Assert.AreEqual("movetime 1000", 
        new MoveTimeModifier(TimeSpan.FromSeconds(1)).Command);
    }

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void TestMoveTimeModifierLessThanZero()
    {
      new MoveTimeModifier(TimeSpan.FromSeconds(-1));
    }

    #endregion

    #region ' MovesToGoModifier '

    [TestMethod]
    public void TestMovesToGo()
    {
      Assert.AreEqual("movestogo 10", new MovesToGoModifier(10).Command);
    }

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void TestMovesToGoLessThanZero()
    {
      new MovesToGoModifier(-1);
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void TestMovesToGoEqualsZero()
    {
      new MovesToGoModifier(0);
    }

    #endregion

    [TestMethod]
    public void PonderModifierTest()
    {
      Assert.AreEqual("ponder", new PonderModifier().Command);
    }

    #region ' SearchMateModifier '

    [TestMethod]
    public void TestSearchMateModifier()
    {
      Assert.AreEqual("mate 10", new SearchMateModifier(10).Command);
    }

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void TestSearchMateModifierLessThanZero()
    {
      new SearchMateModifier(-1);
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void TestSearchMateModifierEqualsZero()
    {
      new SearchMateModifier(0);
    }

    #endregion
    
    #region ' SearchMovesModifier '

    [TestMethod]
    public void TestSearchMovesModifier()
    {
      Assert.AreEqual("searchmoves m1 m2", new SearchMovesModifier("m1", "m2").Command);
    }

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void TestSearchMovesModifierWithNullMove()
    {
      new SearchMovesModifier("m1", null);
    }

    #endregion
  }
}