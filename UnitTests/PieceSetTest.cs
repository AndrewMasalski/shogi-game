using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;
using System.Linq;

namespace UnitTests
{
  [TestClass]
  public class PieceSetTest
  {
    [TestMethod]
    public void TestResetOwnerOnReturn()
    {
      var board = new Board();
      var piece = board.PieceSet["馬"];
      board.PieceSet.Take(piece);
      piece.Owner = board.White;
      board.PieceSet.Return(piece);
      Assert.IsNull(piece.Owner);
    }
    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void TestTakeTwice()
    {
      var board = new Board();
      var piece = board.PieceSet["馬"];
      board.PieceSet.Take(piece);
      board.PieceSet.Take(piece);
    }
    [TestMethod, ExpectedException(typeof(PieceHasNoOwnerException))]
    public void TestSetPieceWithoutSettingOwner()
    {
      var board = new Board();
      var piece = board.PieceSet["馬"];
      board.SetPiece("1i", piece);
    }
    [TestMethod]
    public void TestPromoted()
    {
      var board = new Board();
      var piece = board.PieceSet["馬"];
      Assert.AreEqual("馬", (string)piece.Type);
      Assert.IsNull(piece.Owner);
    }
    [TestMethod]
    public void TestDuplicates()
    {
      var board = new Board();
      var set = new HashSet<Piece>();
      while (true)
      {
        var p = board.PieceSet["馬"];
        if (p == null) break;
        board.PieceSet.Take(p);
        Assert.IsTrue(set.Add(p), "Not all pieces are different objects");
      }
      Assert.IsFalse(ReferenceEquals(set.First(), set.Last()));
    }
    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void TwoPlaces()
    {
      var board = new Board();
      var piece = board.PieceSet["馬"];
      board.SetPiece("1i", piece, PieceColor.White);
      board.White.Hand.Add(piece);
    }
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void TakeNullPiece()
    {
      new Board().PieceSet.Take(null);
    }
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void ReturnNullPiece()
    {
      new Board().PieceSet.Return(null);
    }
    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void ReturnPieceTwice()
    {
      var board = new Board();
      board.PieceSet.Return(board.PieceSet["馬"]);
      
    }
  }
}