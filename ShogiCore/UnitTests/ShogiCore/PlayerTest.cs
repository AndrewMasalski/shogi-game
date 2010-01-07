using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Snapshots;

namespace UnitTests.ShogiCore
{
  [TestClass]
  public class PlayerTest
  {
    [TestMethod]
    public void ResetAllPiecesFromHand()
    {
      var board = new Board();
      board.White.AddToHand("馬");
      board.White.ResetAllPiecesFromHand();
      Assert.AreEqual(0, board.White.Hand.Count);
    }
    [TestMethod]
    public void OpponentPropertyTest()
    {
      var board = new Board();
      Assert.AreSame(board.White, board.Black.Opponent);
      Assert.AreSame(board.Black, board.White.Opponent);
    }
    [TestMethod]
    public void ColorPropertyTest()
    {
      var board = new Board();
      Assert.AreEqual(PieceColor.White, board.White.Color);
      Assert.AreEqual(PieceColor.Black, board.Black.Color);
    }
    [TestMethod]
    public void GetPieceFromHandByTypeMethodTest()
    {
      var player = new Board().White;
      player.AddToHand(PieceType.馬);
      Assert.IsNull(player.GetPieceFromHandByType(PieceType.馬));
      Assert.IsNull(player.GetPieceFromHandByType(PieceType.と));
      Assert.IsNull(player.GetPieceFromHandByType(PieceType.歩));
      Assert.AreSame(player.Hand[0], player.GetPieceFromHandByType(PieceType.角));
    }
    [TestMethod]
    public void AddToHandMethodTest()
    {
      var player = new Board().White;
      var piece = player.AddToHand(PieceType.馬);
      Assert.AreSame(player.Hand[0], piece);
    }
    [TestMethod, ExpectedException(typeof(NotEnoughPiecesInSetException))]
    public void AddToHandPieceYouDontHaveInSetTest()
    {
      var player = new Board().White;
      player.AddToHand(PieceType.馬);
      player.AddToHand(PieceType.馬);
      player.AddToHand(PieceType.馬);
    }
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void LoadNullSnapshotMethodTest()
    {
      new Board().White.LoadHandSnapshot(null);
    }
    [TestMethod]
    public void LoadEmptySnapshotMethodTest()
    {
      var player = new Board().White;
      player.AddToHand(PieceType.馬);
      player.LoadHandSnapshot(new PieceSnapshot[0]);
      Assert.AreEqual(0, player.Hand.Count);
    }
    [TestMethod]
    public void LoadSnapsotMethodTest()
    {
      var player = new Board().White;
      player.AddToHand(PieceType.馬);
      player.LoadHandSnapshot(new [] { new PieceSnapshot(PieceType.金, PieceColor.White), });
      var piece = player.Hand[0];
      Assert.AreEqual(PieceColor.White, piece.Color);
      Assert.AreEqual(PieceType.金, piece.PieceType);
    }
    [TestMethod]
    public void WrongColorLoadSnapsotMethodTest()
    {
      var player = new Board().White;
      player.AddToHand(PieceType.馬);
      player.LoadHandSnapshot(new [] { new PieceSnapshot(PieceType.金, PieceColor.Black), });
      var piece = player.Hand[0];
      Assert.AreEqual(PieceColor.White, piece.Color);
      Assert.AreEqual(PieceType.金, piece.PieceType);
    }
    [TestMethod]
    public void ToStringMethodTest()
    {
      var player = new Board().White;
      Assert.AreEqual("White", player.ToString());
      player.Name = "John";
      Assert.AreEqual("John", player.ToString());
      player.Name = null;
      Assert.AreEqual("White", player.ToString());
    }
    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void AddBusyPieceToHandTest1()
    {
      var player = new Board().White;
      var piece = player.AddToHand(PieceType.歩);
      player.Hand.Add(piece);
    }
    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void AddBusyPieceToHandTest2()
    {
      var board = new Board();
      var piece = board.White.AddToHand(PieceType.歩);
      board.Black.Hand.Add(piece);
    }
    [TestMethod, ExpectedException(typeof(NotSupportedException))]
    public void ClearHandTest()
    {
      new Board().White.Hand.Clear();
    }
    [TestMethod, ExpectedException(typeof(NotSupportedException))]
    public void MovePiecesInHandTest()
    {
      var player = new Board().White;
      player.AddToHand(PieceType.歩);
      player.AddToHand(PieceType.歩);
      player.Hand.Move(0, 1);
    }

  }
}