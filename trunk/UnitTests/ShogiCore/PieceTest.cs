﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;

namespace UnitTests.ShogiCore
{
  [TestClass]
  public class PieceTest
  {
    [TestMethod]
    public void TestPromote()
    {
      var b = new Board();
      var p = b.PieceSet["歩"];
      p.IsPromoted = true;
      Assert.AreEqual("と", (string)p.PieceType);
      p.IsPromoted = false;
      Assert.AreEqual("歩", (string)p.PieceType);
    }
    [TestMethod]
    public void CellTest()
    {
      var b = new Board();
      var c = b[0, 0];
      Assert.IsNull(c.Piece);
      c.ResetPiece();
      Assert.IsNull(c.Piece);
      var piece = b.PieceSet["馬"];
      c.SetPiece(piece, b.White);
      Assert.AreSame(c.Piece, piece);
      c.SetPiece(piece);
      Assert.AreSame(c.Piece, piece);
    }
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void SetNullOwnerCellTest()
    {
      var b = new Board();
      b[0, 0].SetPiece(b.PieceSet["馬"], null);
    }
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void SetNullPieceCellTest()
    {
      var b = new Board();
      b[0, 0].SetPiece(null, b.White);
    }

  }
}