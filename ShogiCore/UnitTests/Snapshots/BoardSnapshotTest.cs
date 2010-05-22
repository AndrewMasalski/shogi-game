using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.PieceSets;
using Yasc.ShogiCore.Primitives;
using Yasc.ShogiCore.Snapshots;

namespace ShogiCore.UnitTests
{
  [TestClass]
  public class BoardSnapshotTest
  {
    [TestMethod]
    public void Immutability()
    {
      var board = new Board(new StandardPieceSet());
      board.LoadSnapshot(BoardSnapshot.InitialPosition);
      var original = board.CurrentSnapshot;
      var clone = new BoardSnapshot(original, new UsualMoveSnapshot(
        PieceColor.White, Position.Parse("3c"), Position.Parse("3d"), false));
      
      Assert.IsNull(clone.GetPieceAt("3c"));
      Assert.IsNotNull(clone.GetPieceAt("3d"));

      Assert.IsNotNull(original.GetPieceAt("3c"));
      Assert.IsNull(original.GetPieceAt("3d"));
    }
    [TestMethod, ExpectedException(typeof(NotSupportedException))]
    public void CellsCollectionIsReadonly()
    {
      var snapshot = new Board(new StandardPieceSet()).CurrentSnapshot;
      ((IList<PieceSnapshot>)snapshot.Cells).Add(
        new PieceSnapshot(PT.馬, PieceColor.Black));
    }
    [TestMethod, ExpectedException(typeof(NotSupportedException))]
    public void BlackHandCollectionIsReadonly()
    {
      var snapshot = new Board(new StandardPieceSet()).CurrentSnapshot;
      ((IList<PieceSnapshot>)snapshot.BlackHand).Add(
        new PieceSnapshot(PT.馬, PieceColor.Black));
    }
    [TestMethod, ExpectedException(typeof(NotSupportedException))]
    public void WhiteHandCollectionIsReadonly()
    {
      var snapshot = new Board(new StandardPieceSet()).CurrentSnapshot;
      ((IList<PieceSnapshot>)snapshot.WhiteHand).Add(
        new PieceSnapshot(PT.馬, PieceColor.Black));
    }
    [TestMethod]
    public void HandIndexerTest()
    {
      var snapshot = new Board(new StandardPieceSet()).CurrentSnapshot;
      Assert.AreSame(snapshot.WhiteHand, snapshot.Hand(PieceColor.White));
      Assert.AreSame(snapshot.BlackHand, snapshot.Hand(PieceColor.Black));
    }
    [TestMethod]
    public void IndexerTest()
    {
      var board = new Board(new StandardPieceSet());
      board.LoadSnapshot(BoardSnapshot.InitialPosition);
      var snapshot = board.CurrentSnapshot;
      foreach (var p in Position.OnBoard)
        Assert.AreSame(snapshot.GetPieceAt(p.X, p.Y), snapshot.GetPieceAt(p));
    }
    [TestMethod]
    public void EqualsTest()
    {
      var board = new Board(new StandardPieceSet());
      board.LoadSnapshot(BoardSnapshot.InitialPosition);
      var snapshot1 = board.CurrentSnapshot;
      var snapshot2 = board.CurrentSnapshot;
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
      var snapshot = new Board(new StandardPieceSet()).CurrentSnapshot;
      Assert.IsFalse(snapshot.IsCheckFor(PieceColor.Black));
    }
    [TestMethod]
    public void SnapshotIsBinarySerializable()
    {
      var formatter = new BinaryFormatter();
      using (var buffer = new MemoryStream())
      {
        var board = new Board(new StandardPieceSet());
        board.LoadSnapshot(BoardSnapshot.InitialPosition);
        formatter.Serialize(buffer, board.CurrentSnapshot);
      }
    }
  }
}