using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.PieceSets;
using Yasc.ShogiCore.Primitives;

namespace ShogiCore.UnitTests.Core
{
  [TestClass]
  public class PieceTest
  {
    private Board _board;

    [TestInitialize]
    public void Init()
    {
      _board = new Board(new StandardPieceSet());
    }
    [TestMethod]
    public void TestPromote()
    {
      var p = _board.PieceSet[PT.歩];
      p.IsPromoted = true;
      Assert.AreEqual(PT.と, p.PieceType);
      p.IsPromoted = false;
      Assert.AreEqual(PT.歩, p.PieceType);
    }
    [TestMethod]
    public void CellTest()
    {
      var c = _board.GetCellAt(0, 0);
      Assert.IsNull(c.Piece);
      _board.ResetPiece(c.Position);
      Assert.IsNull(c.Piece);
      var piece = _board.PieceSet[PT.馬];
      _board.SetPiece(piece, c.Position, _board.White);
      Assert.AreSame(c.Piece, piece);
      _board.SetPiece(piece, c.Position);
      Assert.AreSame(c.Piece, piece);
    }
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void SetNullOwnerCellTest()
    {
      _board.SetPiece(_board.PieceSet[PT.馬], _board.GetCellAt(0, 0).Position, null);
    }
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void SetNullPieceCellTest()
    {
      _board.SetPiece((Piece)null, _board.GetCellAt(0, 0).Position, _board.White);
    }
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void SetNullPieceTypeCellTest()
    {
      _board.SetPiece((IPieceType)null, _board.GetCellAt(0, 0).Position, _board.White);
    }
    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void SetPieceWhichIsAlreadySet()
    {
      var piece = _board.PieceSet[PT.馬];
      _board.SetPiece(piece, _board.GetCellAt(0, 0).Position, _board.White);
      _board.SetPiece(piece, _board.GetCellAt(0, 1).Position, _board.White);
    }
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void SetNullPieceTest()
    {
      _board.SetPiece(null, _board.GetCellAt(0, 0).Position);
    }
    [TestMethod]
    public void SetPieceWithOwner()
    {
      var piece = _board.PieceSet[PT.馬];
      _board.SetPiece(piece, _board.GetCellAt(0, 0).Position, _board.White);
      _board.SetPiece(piece, _board.GetCellAt(0, 1).Position);
    }
    [TestMethod, ExpectedException(typeof(PieceHasNoOwnerException))]
    public void SetOwnerlessPiece()
    {
      var piece = _board.PieceSet[PT.馬];
      _board.SetPiece(piece, _board.GetCellAt(0, 0).Position);
    }
  }
}