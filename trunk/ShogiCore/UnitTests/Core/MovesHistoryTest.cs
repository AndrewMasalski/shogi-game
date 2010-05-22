using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.Notations;
using Yasc.ShogiCore.PieceSets;
using Yasc.ShogiCore.Primitives;

namespace ShogiCore.UnitTests.ShogiCore.Moves
{
  [TestClass]
  public class MovesHistoryTest
  {
    private Board _board;

    [TestInitialize]
    public void Init()
    {
      _board = new Board(new StandardPieceSet());
    }

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
    [TestMethod]
    public void TestDerivativeProps()
    {
      var history = new MovesHistory();
      Assert.IsTrue(history.IsEmpty);
      Assert.IsTrue(history.IsCurrentMoveLast);
      var m1 = CreateDummyMove();
      history.Do(m1);
      Assert.IsFalse(history.IsEmpty);
      Assert.IsTrue(history.IsCurrentMoveLast);
      var m2 = CreateDummyMove();
      history.Do(m2);
      Assert.IsFalse(history.IsEmpty);
      Assert.IsTrue(history.IsCurrentMoveLast);

      history.CurrentMove = m1;
      Assert.IsFalse(history.IsEmpty);
      Assert.IsFalse(history.IsCurrentMoveLast);

      history.CurrentMove = null;
      Assert.AreEqual(-1, history.CurrentMoveIndex);
      Assert.IsFalse(history.IsCurrentMoveLast);

      history.CurrentMoveIndex = 1;
      Assert.AreSame(m2, history.CurrentMove);
      Assert.IsTrue(history.IsCurrentMoveLast);
    }
    private static MoveBase CreateDummyMove()
    {
      return new Board(new StandardPieceSet()).GetUsualMove("1i", "2i", false);
    }
    [TestMethod]
    public void HistoryTest()
    {
      _board.SetPiece(PT.歩, _board.Black, "1i");
      var move = _board.GetMove("1i-1h", FormalNotation.Instance).First();
      var s1 = _board.CurrentSnapshot;
      _board.MakeMove(move);
      var s2 = _board.CurrentSnapshot;
      _board.History.CurrentMoveIndex = -1;
      Assert.AreEqual(s1, _board.CurrentSnapshot);
      _board.History.CurrentMoveIndex = 0;
      Assert.AreEqual(s2, _board.CurrentSnapshot);
    }
  }
}