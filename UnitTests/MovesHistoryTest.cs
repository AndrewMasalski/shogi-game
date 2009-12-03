using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Moves;

namespace UnitTests
{
  [TestClass]
  public class MovesHistoryTest
  {
    [TestMethod]
    public void DefaultTestCase()
    {
      var history = new MovesHistory();
      Assert.AreEqual(-1, history.CurrentMoveIndex);
      Assert.IsNull(history.CurrentMove);
      
      var m1 = CreateDummyMove();
      history.Do(m1);
      Assert.AreEqual(0, history.CurrentMoveIndex);
      Assert.AreSame(m1, history.CurrentMove);
      
      var m2 = CreateDummyMove();
      history.Do(m2);
      Assert.AreEqual(1, history.CurrentMoveIndex);
      Assert.AreSame(m2, history.CurrentMove);
    }

    [TestMethod]
    public void ChangeCurrentTest()
    {
      var history = new MovesHistory();
      var m1 = CreateDummyMove();
      history.Do(m1);
      var m2 = CreateDummyMove();
      history.Do(m2);

      history.CurrentMove = m1;
      Assert.AreEqual(0, history.CurrentMoveIndex);
      
      history.CurrentMove = null;
      Assert.AreEqual(-1, history.CurrentMoveIndex);

      history.CurrentMoveIndex = 1;
      Assert.AreSame(m2, history.CurrentMove);

      history.CurrentMoveIndex = -1;
      Assert.IsNull(history.CurrentMove);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void AddNullTest()
    {
      new MovesHistory().Do(null);
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void SetInvalidIndex()
    {
      new MovesHistory {CurrentMoveIndex = 0};
    }

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void SetInvalidIndex1()
    {
      var history = new MovesHistory();
      history.Do(CreateDummyMove());
      history.CurrentMoveIndex = 1;
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void SetInvalidCurrentMove()
    {
      new MovesHistory {CurrentMove = CreateDummyMove()};
    }

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void SetInvalidCurrentMove1()
    {
      var history = new MovesHistory();
      history.Do(CreateDummyMove());
      history.CurrentMove = CreateDummyMove();
    }
    private static MoveBase CreateDummyMove()
    {
      return UsualMove.Create(new Board(), "1i", "2i", false);
    }
  }
}