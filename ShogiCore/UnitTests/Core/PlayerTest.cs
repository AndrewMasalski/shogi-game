﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore.Core;
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
      var board = new Board();
      board.White.Hand.Add(PieceType.馬);
      board.White.Hand.Clear();
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
    public void GetByTypeMethodTest()
    {
      var player = new Board().White;
      player.Hand.Add(PieceType.馬);
      Assert.IsNull(player.Hand.GetByType(PieceType.馬));
      Assert.IsNull(player.Hand.GetByType(PieceType.と));
      Assert.IsNull(player.Hand.GetByType(PieceType.歩));
      Assert.AreSame(player.Hand[0], player.Hand.GetByType(PieceType.角));
    }
    [TestMethod]
    public void AddToHandMethodTest()
    {
      var player = new Board().White;
      var piece = player.Hand.Add(PieceType.馬);
      Assert.AreSame(player.Hand[0], piece);
    }
    [TestMethod, ExpectedException(typeof(NotEnoughPiecesInSetException))]
    public void AddToHandPieceYouDontHaveInSetTest()
    {
      var player = new Board().White;
      player.Hand.Add(PieceType.馬);
      player.Hand.Add(PieceType.馬);
      player.Hand.Add(PieceType.馬);
    }
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void LoadNullSnapshotMethodTest()
    {
      new Board().White.Hand.LoadSnapshot(null);
    }
    [TestMethod]
    public void LoadEmptySnapshotMethodTest()
    {
      var player = new Board().White;
      player.Hand.Add(PieceType.馬);
      player.Hand.LoadSnapshot(new PieceSnapshot[0]);
      Assert.AreEqual(0, player.Hand.Count);
    }
    [TestMethod]
    public void LoadSnapsotMethodTest()
    {
      var player = new Board().White;
      player.Hand.Add(PieceType.馬);
      player.Hand.LoadSnapshot(new [] { new PieceSnapshot(PieceType.金, PieceColor.White), });
      var piece = player.Hand[0];
      Assert.AreEqual(PieceColor.White, piece.Color);
      Assert.AreEqual(PieceType.金, piece.PieceType);
    }
    [TestMethod]
    public void WrongColorLoadSnapsotMethodTest()
    {
      var player = new Board().White;
      player.Hand.Add(PieceType.馬);
      player.Hand.LoadSnapshot(new [] { new PieceSnapshot(PieceType.金, PieceColor.Black), });
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
      var piece = player.Hand.Add(PieceType.歩);
      player.Hand.Add(piece);
    }
    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void AddBusyPieceToHandTest2()
    {
      var board = new Board();
      var piece = board.White.Hand.Add(PieceType.歩);
      board.Black.Hand.Add(piece);
    }
  }
}