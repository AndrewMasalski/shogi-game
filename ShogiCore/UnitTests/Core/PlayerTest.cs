using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.PieceSets;
using Yasc.ShogiCore.Primitives;
using Yasc.ShogiCore.Snapshots;

namespace ShogiCore.UnitTests.ShogiCore
{
  [TestClass]
  public class PlayerTest
  {
    [TestMethod]
    public void ResetAllPiecesFromHand()
    {
      var board = new Board(new StandardPieceSet());
      board.White.Hand.Add(PT.馬);
      board.White.Hand.Clear();
      Assert.AreEqual(0, board.White.Hand.Count);
    }
    [TestMethod]
    public void OpponentPropertyTest()
    {
      var board = new Board(new StandardPieceSet());
      Assert.AreSame(board.White, board.Black.Opponent);
      Assert.AreSame(board.Black, board.White.Opponent);
    }
    [TestMethod]
    public void ColorPropertyTest()
    {
      var board = new Board(new StandardPieceSet());
      Assert.AreEqual(PieceColor.White, board.White.Color);
      Assert.AreEqual(PieceColor.Black, board.Black.Color);
    }
    [TestMethod]
    public void GetByTypeMethodTest()
    {
      var player = new Board(new StandardPieceSet()).White;
      player.Hand.Add(PT.馬);
      Assert.IsNull(player.Hand.GetByType(PT.馬));
      Assert.IsNull(player.Hand.GetByType(PT.と));
      Assert.IsNull(player.Hand.GetByType(PT.歩));
      Assert.AreSame(player.Hand[0], player.Hand.GetByType(PT.角));
    }
    [TestMethod]
    public void AddToHandMethodTest()
    {
      var player = new Board(new StandardPieceSet()).White;
      var piece = player.Hand.Add(PT.馬);
      Assert.AreSame(player.Hand[0], piece);
    }
    [TestMethod, ExpectedException(typeof(NotEnoughPiecesInSetException))]
    public void AddToHandPieceYouDontHaveInSetTest()
    {
      var player = new Board(new StandardPieceSet()).White;
      player.Hand.Add(PT.馬);
      player.Hand.Add(PT.馬);
      player.Hand.Add(PT.馬);
    }
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void LoadNullSnapshotMethodTest()
    {
      new Board(new StandardPieceSet()).White.Hand.LoadSnapshot(null);
    }
    [TestMethod]
    public void LoadEmptySnapshotMethodTest()
    {
      var player = new Board(new StandardPieceSet()).White;
      player.Hand.Add(PT.馬);
      player.Hand.LoadSnapshot(new PieceSnapshot[0]);
      Assert.AreEqual(0, player.Hand.Count);
    }
    [TestMethod]
    public void LoadSnapsotMethodTest()
    {
      var player = new Board(new StandardPieceSet()).White;
      player.Hand.Add(PT.馬);
      player.Hand.LoadSnapshot(new [] { new PieceSnapshot(PT.金, PieceColor.White), });
      var piece = player.Hand[0];
      Assert.AreEqual(PieceColor.White, piece.Color);
      Assert.AreEqual(PT.金, piece.PieceType);
    }
    [TestMethod]
    public void WrongColorLoadSnapsotMethodTest()
    {
      var player = new Board(new StandardPieceSet()).White;
      player.Hand.Add(PT.馬);
      player.Hand.LoadSnapshot(new [] { new PieceSnapshot(PT.金, PieceColor.Black), });
      var piece = player.Hand[0];
      Assert.AreEqual(PieceColor.White, piece.Color);
      Assert.AreEqual(PT.金, piece.PieceType);
    }
    [TestMethod]
    public void ToStringMethodTest()
    {
      var player = new Board(new StandardPieceSet()).White;
      Assert.AreEqual("White", player.ToString());
      player.Name = "John";
      Assert.AreEqual("John", player.ToString());
      player.Name = null;
      Assert.AreEqual("White", player.ToString());
    }
    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void AddBusyPieceToHandTest1()
    {
      var player = new Board(new StandardPieceSet()).White;
      var piece = player.Hand.Add(PT.歩);
      player.Hand.Add(piece);
    }
    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void AddBusyPieceToHandTest2()
    {
      var board = new Board(new StandardPieceSet());
      var piece = board.White.Hand.Add(PT.歩);
      board.Black.Hand.Add(piece);
    }
  }
}