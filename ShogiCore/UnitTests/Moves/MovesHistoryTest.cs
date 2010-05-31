using System;
using System.Linq;
using CommonUtils.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.Moves;
using Yasc.ShogiCore.Notations;
using Yasc.ShogiCore.PieceSets;
using Yasc.ShogiCore.Primitives;
using Yasc.ShogiCore.Snapshots;

namespace ShogiCore.UnitTests.Moves
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
      history.Add(m1);
      Assert.AreEqual(0, history.CurrentMoveIndex);
      Assert.AreSame(m1, history.CurrentMove);
      
      var m2 = CreateDummyMove();
      history.Add(m2);
      Assert.AreEqual(1, history.CurrentMoveIndex);
      Assert.AreSame(m2, history.CurrentMove);
    }

    [TestMethod]
    public void ChangeCurrentTest()
    {
      var history = new MovesHistory();
      var m1 = CreateDummyMove();
      history.Add(m1);
      var m2 = CreateDummyMove();
      history.Add(m2);

      history.CurrentMove = m1;
      Assert.AreEqual(0, history.CurrentMoveIndex);
      
      history.CurrentMove = null;
      Assert.AreEqual(-1, history.CurrentMoveIndex);

      history.CurrentMoveIndex = 1;
      Assert.AreSame(m2, history.CurrentMove);

      history.CurrentMoveIndex = -1;
      Assert.IsNull(history.CurrentMove);
    }

    [TestMethod]
    public void ExpectedExceptions()
    {
      MyAssert.ThrowsException<ArgumentNullException>(
        () => new MovesHistory().Add((DecoratedMove)null));

      MyAssert.ThrowsException<ArgumentNullException>(
        () => new MovesHistory().Add((Move)null));

      MyAssert.ThrowsException<ArgumentOutOfRangeException>(
        () => new MovesHistory { CurrentMoveIndex = 0 });

      MyAssert.ThrowsException<ArgumentOutOfRangeException>(
        () =>
          {
            var history = new MovesHistory { CreateDummyMove() };
            history.CurrentMoveIndex = 1;            
          });

      MyAssert.ThrowsException<ArgumentOutOfRangeException>(
        () => new MovesHistory { CurrentMove = CreateDummyMove() });

      MyAssert.ThrowsException<ArgumentOutOfRangeException>(
        () =>
        {
          var history = new MovesHistory { CreateDummyMove() };
          history.CurrentMove = CreateDummyMove();
        });

    }
    
    [TestMethod]
    public void TestDerivativeProps()
    {
      var history = new MovesHistory();
      Assert.IsTrue(history.IsEmpty);
      Assert.IsTrue(history.IsCurrentMoveLast);
      var m1 = CreateDummyMove();
      history.Add(m1);
      Assert.IsFalse(history.IsEmpty);
      Assert.IsTrue(history.IsCurrentMoveLast);
      var m2 = CreateDummyMove();
      history.Add(m2);
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
    private static DecoratedMove CreateDummyMove()
    {
      var board = new Board(new StandardPieceSet());
      return board.History.Decorate(board.GetUsualMove("1i", "2i"));
    }

    [TestMethod]
    public void HistoryTest()
    {
      _board.SetPiece(PT.歩, "1i", _board.Black);
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