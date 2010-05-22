using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.Primitives;

namespace ShogiCore.UnitTests.ShogiCore
{
  [TestClass]
  public class PieceTest
  {
    [TestMethod]
    public void TestPromote()
    {
      var b = new Board();
      var p = b.PieceSet[PieceType.歩];
      p.IsPromoted = true;
      Assert.AreEqual(PieceType.と, p.PieceType);
      p.IsPromoted = false;
      Assert.AreEqual(PieceType.歩, p.PieceType);
    }
    [TestMethod]
    public void CellTest()
    {
      var b = new Board();
      var c = b.GetCellAt(0, 0);
      Assert.IsNull(c.Piece);
      b.ResetPiece(c.Position);
      Assert.IsNull(c.Piece);
      var piece = b.PieceSet[PieceType.馬];
      b.SetPiece(c.Position, piece, b.White);
      Assert.AreSame(c.Piece, piece);
      b.SetPiece(c.Position, piece);
      Assert.AreSame(c.Piece, piece);
    }
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void SetNullOwnerCellTest()
    {
      var b = new Board();
      b.SetPiece(b.GetCellAt(0, 0).Position, b.PieceSet[PieceType.馬], null);
    }
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void SetNullPieceCellTest()
    {
      var b = new Board();
      b.SetPiece(b.GetCellAt(0, 0).Position, null, b.White);
    }
    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void SetPieceWhichIsAlreadySet()
    {
      var b = new Board();
      var piece = b.PieceSet[PieceType.馬];
      b.SetPiece(b.GetCellAt(0, 0).Position, piece, b.White);
      b.SetPiece(b.GetCellAt(0, 1).Position, piece, b.White);
    }
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void SetNullPieceTest()
    {
      var b = new Board();
      b.SetPiece(b.GetCellAt(0, 0).Position, null);
    }
    [TestMethod]
    public void SetPieceWithOwner()
    {
      var b = new Board();
      var piece = b.PieceSet[PieceType.馬];
      b.SetPiece(b.GetCellAt(0, 0).Position, piece, b.White);
      b.SetPiece(b.GetCellAt(0, 1).Position, piece);
    }
    [TestMethod, ExpectedException(typeof(PieceHasNoOwnerException))]
    public void SetOwnerlessPiece()
    {
      var b = new Board();
      var piece = b.PieceSet[PieceType.馬];
      b.SetPiece(b.GetCellAt(0, 0).Position, piece);
    }
  }
}