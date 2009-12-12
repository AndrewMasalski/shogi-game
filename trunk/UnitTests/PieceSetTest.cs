using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;

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
    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void TwoPlaces()
    {
      var board = new Board();
      var piece = board.PieceSet["馬"];
      board.SetPiece("1i", piece, PieceColor.White);
      board.White.Hand.Add(piece);
    }
  }
}