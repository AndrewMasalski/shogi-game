using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.PieceSets;
using Yasc.ShogiCore.Primitives;

namespace ShogiCore.UnitTests.Core
{
  [TestClass]
  public class PieceSetTest
  {
    [TestMethod]
    public void TestResetOwnerOnReturn()
    {
      var board = new Board(new StandardPieceSet());
      var piece = board.PieceSet[PT.馬];
      board.PieceSet.AcquirePiece(piece);
      piece.Owner = board.White;
      board.PieceSet.ReleasePiece(piece);
      Assert.IsNull(piece.Owner);
    }
    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void TestTakeTwice()
    {
      var board = new Board(new StandardPieceSet());
      var piece = board.PieceSet[PT.馬];
      board.PieceSet.AcquirePiece(piece);
      board.PieceSet.AcquirePiece(piece);
    }
    [TestMethod]
    public void TestPromoted()
    {
      var board = new Board(new StandardPieceSet());
      var piece = board.PieceSet[PT.馬];
      Assert.AreEqual(PT.馬, piece.PieceType);
      Assert.IsNull(piece.Owner);
    }
    [TestMethod]
    public void TestDuplicates()
    {
      var board = new Board(new StandardPieceSet());
      var set = new HashSet<Piece>();
      while (true)
      {
        var p = board.PieceSet[PT.馬];
        if (p == null) break;
        board.PieceSet.AcquirePiece(p);
        Assert.IsTrue(set.Add(p), "Not all pieces are different objects");
      }
      Assert.IsFalse(ReferenceEquals(set.First(), set.Last()));
    }
    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void TwoPlaces()
    {
      var board = new Board(new StandardPieceSet());
      var piece = board.PieceSet[PT.馬];
      board.SetPiece(piece, "1i", PieceColor.White);
      board.White.Hand.Add(piece);
    }
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void TakeNullPiece()
    {
      new Board(new StandardPieceSet()).PieceSet.AcquirePiece(null);
    }
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void ReturnNullPiece()
    {
      new Board(new StandardPieceSet()).PieceSet.ReleasePiece(null);
    }
    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void ReturnPieceTwice()
    {
      var board = new Board(new StandardPieceSet());
      board.PieceSet.ReleasePiece(board.PieceSet[PT.馬]);
      
    }
  }
}