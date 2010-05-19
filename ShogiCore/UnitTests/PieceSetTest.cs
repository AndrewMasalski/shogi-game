﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.Primitives;

namespace ShogiCore.UnitTests
{
  [TestClass]
  public class PieceSetTest
  {
    [TestMethod]
    public void TestResetOwnerOnReturn()
    {
      var board = new Board();
      var piece = board.PieceSet[(PieceType)"馬"];
      board.PieceSet.Pop(piece);
      piece.Owner = board.White;
      board.PieceSet.Push(piece);
      Assert.IsNull(piece.Owner);
    }
    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void TestTakeTwice()
    {
      var board = new Board();
      var piece = board.PieceSet[(PieceType)"馬"];
      board.PieceSet.Pop(piece);
      board.PieceSet.Pop(piece);
    }
    [TestMethod]
    public void TestPromoted()
    {
      var board = new Board();
      var piece = board.PieceSet[(PieceType)"馬"];
      Assert.AreEqual((PieceType)"馬", (string)piece.PieceType);
      Assert.IsNull(piece.Owner);
    }
    [TestMethod]
    public void TestDuplicates()
    {
      var board = new Board();
      var set = new HashSet<Piece>();
      while (true)
      {
        var p = board.PieceSet[(PieceType)"馬"];
        if (p == null) break;
        board.PieceSet.Pop(p);
        Assert.IsTrue(set.Add(p), "Not all pieces are different objects");
      }
      Assert.IsFalse(ReferenceEquals(set.First(), set.Last()));
    }
    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void TwoPlaces()
    {
      var board = new Board();
      var piece = board.PieceSet[(PieceType)"馬"];
      board.SetPiece(piece, PieceColor.White, "1i");
      board.White.Hand.Add(piece);
    }
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void TakeNullPiece()
    {
      new Board().PieceSet.Pop(null);
    }
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void ReturnNullPiece()
    {
      new Board().PieceSet.Push(null);
    }
    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void ReturnPieceTwice()
    {
      var board = new Board();
      board.PieceSet.Push(board.PieceSet[(PieceType)"馬"]);
      
    }
  }
}