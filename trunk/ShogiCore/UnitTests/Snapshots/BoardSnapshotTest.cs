using System;
using System.Collections.Generic;
using CommonUtils.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.PieceSets;
using Yasc.ShogiCore.Primitives;
using Yasc.ShogiCore.Snapshots;

namespace ShogiCore.UnitTests.Snapshots
{
  [TestClass]
  public class BoardSnapshotTest
  {
    private BoardSnapshot _snapshot;

    [TestInitialize]
    public void Init()
    {
      var board = new Board(new StandardPieceSet());
      board.LoadSnapshot(BoardSnapshot.InitialPosition);
      _snapshot = board.CurrentSnapshot;
    }

    [TestMethod]
    public void Immutability()
    {
      var clone = _snapshot.MakeMove(new UsualMove(_snapshot,
        PieceColor.White, Position.Parse("3g"), Position.Parse("3f"), false));
      
      Assert.IsNull(clone.GetPieceAt("3g"));
      Assert.IsNotNull(clone.GetPieceAt("3f"));

      Assert.IsNotNull(_snapshot.GetPieceAt("3g"));
      Assert.IsNull(_snapshot.GetPieceAt("3f"));
    }
    [TestMethod]
    public void CollectionsReadonly()
    {
      MyAssert.ThrowsException<NotSupportedException>(
        () => ((IList<IColoredPiece>)_snapshot.Cells).Add(PT.馬.Black));

      MyAssert.ThrowsException<NotSupportedException>(
        () => ((IList<IPieceType>)_snapshot.BlackHand).Add(PT.馬));

      MyAssert.ThrowsException<NotSupportedException>(
        () => ((IList<IPieceType>)_snapshot.WhiteHand).Add(PT.馬));
    }
    [TestMethod]
    public void HandIndexerTest()
    {
      Assert.AreSame(_snapshot.WhiteHand, _snapshot.GetHand(PieceColor.White));
      Assert.AreSame(_snapshot.BlackHand, _snapshot.GetHand(PieceColor.Black));
    }
    [TestMethod]
    public void GetPieceAtOverloadsTest()
    {
      foreach (var p in Position.OnBoard)
        Assert.AreSame(
          _snapshot.Cells[p.X, p.Y], 
          _snapshot.GetPieceAt(p));
    }
    [TestMethod]
    public void EqualityTest()
    {
      var snapshot1 = _snapshot;
      var snapshot2 = _snapshot.SerializeDeserialize();

      Assert.AreNotSame(snapshot1, snapshot2);
      Assert.IsTrue(snapshot1.Equals(snapshot2));
      Assert.IsTrue(snapshot1.Equals((object)snapshot2));
      Assert.IsFalse(snapshot1.Equals(new object()));
      Assert.IsFalse(snapshot1.Equals(null));
      Assert.IsTrue(snapshot1 == snapshot2);
      Assert.IsFalse(snapshot1 != snapshot2);
    }
    [TestMethod]
    public void IsCheckForTest()
    {
      Assert.IsFalse(_snapshot.IsCheckFor(PieceColor.Black));
    }
    [TestMethod]
    public void ParseSfenExceptions()
    {
      MyAssert.ThrowsException<ArgumentNullException>(
        () => BoardSnapshot.ParseSfen(null));

      MyAssert.ThrowsException<ArgumentOutOfRangeException>(
        () => BoardSnapshot.ParseSfen(""));

      MyAssert.ThrowsException<ArgumentOutOfRangeException>(
        () => BoardSnapshot.ParseSfen("a b c"));
    }

    [TestMethod]
    public void ParseSfen()
    {
      var boardSnapshot = BoardSnapshot.ParseSfen("lnsgkgsnl/1r5b1/ppppppppp/9/9/9/PPPPPPPPP/1B5R1/LNSGKGSNL w - 1");
      Assert.AreEqual(BoardSnapshot.InitialPosition, boardSnapshot);
    }
  }
}