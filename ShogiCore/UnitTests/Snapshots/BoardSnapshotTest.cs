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
      var clone = new BoardSnapshot(_snapshot, new UsualMove(
        PieceColor.White, Position.Parse("3c"), Position.Parse("3d"), false));
      
      Assert.IsNull(clone.GetPieceAt("3c"));
      Assert.IsNotNull(clone.GetPieceAt("3d"));

      Assert.IsNotNull(_snapshot.GetPieceAt("3c"));
      Assert.IsNull(_snapshot.GetPieceAt("3d"));
    }
    [TestMethod, ExpectedException(typeof(NotSupportedException))]
    public void CellsCollectionIsReadonly()
    {
      ((IList<IColoredPiece>)_snapshot.Cells).Add(PT.馬.Black);
    }
    [TestMethod, ExpectedException(typeof(NotSupportedException))]
    public void BlackHandCollectionIsReadonly()
    {
      ((IList<IPieceType>)_snapshot.BlackHand).Add(PT.馬);
    }
    [TestMethod, ExpectedException(typeof(NotSupportedException))]
    public void WhiteHandCollectionIsReadonly()
    {
      ((IList<IPieceType>)_snapshot.WhiteHand).Add(PT.馬);
    }
    [TestMethod]
    public void HandIndexerTest()
    {
      Assert.AreSame(_snapshot.WhiteHand, _snapshot.Hand(PieceColor.White));
      Assert.AreSame(_snapshot.BlackHand, _snapshot.Hand(PieceColor.Black));
    }
    [TestMethod]
    public void GetPieceAtOverloadsTest()
    {
      foreach (var p in Position.OnBoard)
        Assert.AreSame(
          _snapshot.GetPieceAt(p.X, p.Y), 
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
  }
}