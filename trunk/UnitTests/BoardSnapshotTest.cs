using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;
using Yasc.ShogiCore.SnapShots;
using Yasc.ShogiCore.Utils;

namespace UnitTests
{
  [TestClass]
  public class BoardSnapshotTest
  {
    [TestMethod]
    public void Immutability()
    {
      var board = new Board();
      Shogi.InititBoard(board);
      var original = new BoardSnapshot(board);
      var clone = new BoardSnapshot(original,
        new UsualMoveSnapshot("3c", "3d", false));
      
      Assert.IsNull(clone["3c"]);
      Assert.IsNotNull(clone["3d"]);

      Assert.IsNotNull(original["3c"]);
      Assert.IsNull(original["3d"]);
    }
    [TestMethod, ExpectedException(typeof(NotSupportedException))]
    public void CellsCollectionIsReadonly()
    {
      var snapshot = new BoardSnapshot(new Board());
      ((IList<PieceSnapshot>)snapshot.Cells).Add(
        new PieceSnapshot(PieceType.馬, PieceColor.Black));
    }
    [TestMethod, ExpectedException(typeof(NotSupportedException))]
    public void BlackHandCollectionIsReadonly()
    {
      var snapshot = new BoardSnapshot(new Board());
      ((IList<PieceSnapshot>)snapshot.BlackHand).Add(
        new PieceSnapshot(PieceType.馬, PieceColor.Black));
    }
    [TestMethod, ExpectedException(typeof(NotSupportedException))]
    public void WhiteHandCollectionIsReadonly()
    {
      var snapshot = new BoardSnapshot(new Board());
      ((IList<PieceSnapshot>)snapshot.WhiteHand).Add(
        new PieceSnapshot(PieceType.馬, PieceColor.Black));
    }
    [TestMethod]
    public void HandIndexerTest()
    {
      var snapshot = new BoardSnapshot(new Board());
      Assert.AreSame(snapshot.WhiteHand, snapshot.Hand(PieceColor.White));
      Assert.AreSame(snapshot.BlackHand, snapshot.Hand(PieceColor.Black));
    }
    [TestMethod]
    public void IndexerTest()
    {
      var board = new Board();
      Shogi.InititBoard(board);
      var snapshot = new BoardSnapshot(board);
      foreach (var p in Position.OnBoard)
        Assert.AreSame(snapshot[p.X, p.Y], snapshot[p]);
    }
    [TestMethod]
    public void EqualsTest()
    {
      var board = new Board();
      Shogi.InititBoard(board);
      var snapshot1 = new BoardSnapshot(board);
      var snapshot2 = new BoardSnapshot(board);
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
      var snapshot = new BoardSnapshot(new Board());
      Assert.IsFalse(snapshot.IsCheckFor(PieceColor.Black));
    }
  }
}