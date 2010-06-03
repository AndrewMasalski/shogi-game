using CommonUtils.UnitTests;
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
    private Piece _piece;
    // TODO: PropertyObserverAssertion!
    [TestInitialize]
    public void Init()
    {
      _board = new Board(new StandardPieceSet());
      _piece = _board.PieceSet[PT.銀];
    }
    [TestMethod]
    public void IsPromoted()
    {
      _piece.IsPromoted = true;
      Assert.AreEqual(PT.全, _piece.PieceType);
      _piece.IsPromoted = false;
      Assert.AreEqual(PT.銀, _piece.PieceType);
    }
    [TestMethod]
    public void Owner()
    {
      Assert.IsNull(_piece.Owner);
      _board.White.Hand.Add(_piece);
      Assert.IsNotNull(_piece.Owner);
      _board.White.Hand.Clear();
      Assert.IsNull(_piece.Owner);
    }
    [TestMethod]
    public void WhatProhibitedOnOwnerlessPiece()
    {
      MyAssert.ThrowsException<PieceHasNoOwnerException>(
        () => _piece.Color.ToString());

      MyAssert.ThrowsException<PieceHasNoOwnerException>(
        () => _piece.ColoredPiece.ToString());
    }
    [TestMethod]
    public void ToStringTest()
    {
      Assert.AreEqual("Ownerless 銀", _piece.ToString());
    }
    [TestMethod]
    public void ToLatinString()
    {
      Assert.AreEqual("Ownerless S", _piece.ToLatinString());
    }
    [TestMethod]
    public void Snapshot()
    {
      _board.White.Hand.Add(_piece);
      var coloredPiece = _piece.ColoredPiece;
      Assert.AreEqual(PieceColor.White, coloredPiece.Color);
      Assert.AreEqual(PT.銀, coloredPiece.PieceType);
    }
  }
}