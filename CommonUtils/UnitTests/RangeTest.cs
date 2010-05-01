using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.Utils;
using System.Linq;

namespace CommonUtils.UnitTests
{
  [TestClass, Ignore]
  public class RangeTest
  {
    private readonly int[] _list = new[] {1, 2, 3, 4, 2, 3, 4, 5};
    private readonly int[] _other = new[] {4, 2, 3, 4, 5, 1, 2, 3};
  
    [TestMethod]
    public void CtorTest()
    {
      CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 2, 3, 4, 5 }, _list.Range().ToList());
      CollectionAssert.AreEqual(new[] { 3, 4, 5 }, _list.Range(5).ToList());
      CollectionAssert.AreEqual(new[] { 1, 2, 3 }, _list.Range(0, 3).ToList());
      CollectionAssert.AreEqual(new[] { 2, 3 }, _list.Range(1, 2).ToList());
      CollectionAssert.AreEqual(new[] { 2 }, new Range<int>(_list, 4, 1).ToList());
      CollectionAssert.AreEqual(new[] { 2, 3, 4, 5 }, new Range<int>(_list, 4, 4).ToList());
    }
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void CtorNullListTest()
    {
      new Range<int>(null, 0, 1);
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void CtorZeroWidthTest()
    {
      new Range<int>(_list, 0, 0);
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void CtorExceedWidthTest()
    {
      new Range<int>(_list, 0, 9);
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void CtorNegativeIndexTest()
    {
      new Range<int>(_list, -1, 2);
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void CtorBigSumIndexTest()
    {
      new Range<int>(_list, 2, 7);
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void CtorBigIndexTest()
    {
      new Range<int>(_list, 8, 7);
    }

    [TestMethod]
    public void EmptyTest()
    {
      Assert.IsTrue(new Range<int>().IsEmpty);
      Assert.IsTrue(_list.Range().EmptySubrange().IsEmpty);
    }

    [TestMethod]
    public void MiddleTest()
    {
      // 0, 1, 2, 3
      var evenRange = _list.Range(0, 4);
      Assert.AreEqual(1, evenRange.MiddleSubrange(1).Index);
      Assert.AreEqual(1, evenRange.MiddleSubrange(2).Index);
      Assert.AreEqual(0, evenRange.MiddleSubrange(3).Index);
      Assert.AreEqual(0, evenRange.MiddleSubrange(4).Index);

      Assert.AreEqual(1, evenRange.MiddleSubrange(1).Width);
      Assert.AreEqual(2, evenRange.MiddleSubrange(2).Width);
      Assert.AreEqual(3, evenRange.MiddleSubrange(3).Width);
      Assert.AreEqual(4, evenRange.MiddleSubrange(4).Width);

      // 0, 1, 2, 3, 4
      var oddRange = _list.Range(0, 5);
      Assert.AreEqual(2, oddRange.MiddleSubrange(1).Index);
      Assert.AreEqual(1, oddRange.MiddleSubrange(2).Index);
      Assert.AreEqual(1, oddRange.MiddleSubrange(3).Index);
      Assert.AreEqual(0, oddRange.MiddleSubrange(4).Index);
      Assert.AreEqual(0, oddRange.MiddleSubrange(5).Index);

      Assert.AreEqual(1, oddRange.MiddleSubrange(1).Width);
      Assert.AreEqual(2, oddRange.MiddleSubrange(2).Width);
      Assert.AreEqual(3, oddRange.MiddleSubrange(3).Width);
      Assert.AreEqual(4, oddRange.MiddleSubrange(4).Width);
      Assert.AreEqual(5, oddRange.MiddleSubrange(5).Width);
    }

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void BigSizeMiddleTest()
    {
      _list.Range(0, 5).MiddleSubrange(6);
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void ZeroSizeMiddleTest()
    {
      _list.Range(0, 5).MiddleSubrange(0);
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void NegativeSizeMiddleTest()
    {
      _list.Range(0, 5).MiddleSubrange(-6);
    }

    [TestMethod]
    public void SameBaseListFindTest()
    {
      var original = _list.Range(2, 3);
      var found = _list.Range().Find(original, Compare);
      Assert.IsTrue(found.SameAs(original));
      Assert.IsTrue(found.Equals(original));

      original = _list.Range(4, 2);
      found = _list.Range().Find(original, Compare);
      Assert.IsFalse(found.SameAs(original));
      Assert.IsTrue(found.SameAs(_list.Range(1, 2)));
      Assert.IsTrue(found.Equals(original));

      Assert.IsTrue(_list.Range().SameAs(
        _list.Range().Find(_list.Range(), Compare)));

      Assert.IsTrue(_list.Range().Find(
        _list.Range(), (x, y) => false).IsEmpty);
    }
    [TestMethod]
    public void DifferentBaseListsFindTest()
    {
      var original = _list.Range(0, 4);
      var found = _other.Range().Find(original, Compare);
      Assert.IsFalse(found.SameAs(original));
      Assert.IsFalse(found.Equals(original));
      Assert.IsTrue(found.IsEmpty);
      
      original = _list.Range(0, 3);
      found = _other.Range().Find(original, Compare);
      Assert.IsTrue(found.SameAs(_other.Range(5, 3)));
    }

    [TestMethod]
    public void BreakTest()
    {
      RangesCollectionAssert.AreSame(
        new [] {_list.Range(0, 3), _list.Range(6, 2), }, 
        _list.Range().Break(_list.Range(3, 3)));

      RangesCollectionAssert.AreSame(
        new [] {_list.Range(6, 2), }, 
        _list.Range().Break(_list.Range(0, 6)));

      RangesCollectionAssert.AreSame(
        new [] {_list.Range(0, 3), }, 
        _list.Range().Break(_list.Range(3)));

      RangesCollectionAssert.AreSame(
        new Range<int>[0], 
        _list.Range().Break(_list.Range()));
    }
    [TestMethod]
    public void BreakOddTest()
    {
      RangesCollectionAssert.AreSame(
        new [] {_list.Range(1, 2), _list.Range(6, 2), }, 
        _list.Range(1).Break(_list.Range(3, 3)));

      RangesCollectionAssert.AreSame(
        new [] {_list.Range(6, 2), }, 
        _list.Range(1).Break(_list.Range(1, 5)));

      RangesCollectionAssert.AreSame(
        new [] {_list.Range(1, 2), }, 
        _list.Range(1).Break(_list.Range(3)));

      RangesCollectionAssert.AreSame(
        new Range<int>[0], 
        _list.Range(1).Break(_list.Range(1)));
    }
    [TestMethod]
    public void BreakEvenTest()
    {
      RangesCollectionAssert.AreSame(
        new [] {_list.Range(2, 1), _list.Range(6, 2), }, 
        _list.Range(2).Break(_list.Range(3, 3)));

      RangesCollectionAssert.AreSame(
        new [] {_list.Range(6, 2), }, 
        _list.Range(2).Break(_list.Range(2, 4)));

      RangesCollectionAssert.AreSame(
        new [] {_list.Range(2, 1), }, 
        _list.Range(2).Break(_list.Range(3)));

      RangesCollectionAssert.AreSame(
        new Range<int>[0], 
        _list.Range(2).Break(_list.Range(2)));
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void BreakWithEmptyRangeTest()
    {
      _list.Range().Break(new Range<int>());
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void BreakWithBiggerRangeTest()
    {
      _list.Range(3).Break(_list.Range());
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void BreakWithDifferentRangeTest()
    {
      _list.Range(3).Break(_list.Range(0, 1));
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void BreakWithIntersectRangeTest()
    {
      _list.Range(3).Break(_list.Range(0, 5));
    }

    private static bool Compare(int t, int u)
    {
      return t == u;
    }
  }
}