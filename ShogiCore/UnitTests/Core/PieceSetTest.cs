using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.PieceSets;
using Yasc.ShogiCore.Primitives;

namespace ShogiCore.UnitTests.Core
{
  [TestClass]
  public class PieceSetTest
  {
    private Board _board;

    [TestInitialize]
    public void Init()
    {
      _board = new Board(new StandardPieceSet());
    }
    [TestMethod]
    public void CellTest()
    {
      var cell = _board.GetCellAt(0, 0);
      Assert.IsNull(cell.Piece);
      _board.ResetPiece(cell.Position);
      Assert.IsNull(cell.Piece);
      var piece = _board.PieceSet[PT.馬];
      _board.SetPiece(piece, cell.Position, _board.White);
      Assert.AreSame(cell.Piece, piece);
      _board.SetPiece(piece, cell.Position);
      Assert.AreSame(cell.Piece, piece);
    }
    [TestMethod]
    public void TestResetOwnerOnReturn()
    {
      var piece = _board.PieceSet[PT.馬];
      _board.PieceSet.AcquirePiece(piece);
      piece.Owner = _board.White;
      _board.PieceSet.ReleasePiece(piece);
      Assert.IsNull(piece.Owner);
    }
    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void TestTakeTwice()
    {
      var piece = _board.PieceSet[PT.馬];
      _board.PieceSet.AcquirePiece(piece);
      _board.PieceSet.AcquirePiece(piece);
    }
    [TestMethod]
    public void TestPromoted()
    {
      var piece = _board.PieceSet[PT.馬];
      Assert.AreEqual(PT.馬, piece.PieceType);
      Assert.IsNull(piece.Owner);
    }
    [TestMethod]
    public void TestDuplicates()
    {
      var set = new HashSet<Piece>();
      while (true)
      {
        var p = _board.PieceSet[PT.馬];
        if (p == null) break;
        _board.PieceSet.AcquirePiece(p);
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
      _board.PieceSet.ReleasePiece(_board.PieceSet[PT.馬]);
      
    }
    [TestMethod, ExpectedException(typeof(NotEnoughPiecesInSetException))]
    public void CantLoadSnapshotBecauseNotEnoughPiecesTest()
    {
      var board = new Board(InfinitePieceSet.Instance);
      board.SetPiece(PT.馬, "1i", PieceColor.Black);
      board.SetPiece(PT.馬, "2i", PieceColor.Black);
      board.SetPiece(PT.馬, "3i", PieceColor.Black);

      _board.LoadSnapshot(board.CurrentSnapshot);
    }

    [TestMethod]
    public void SetPieceArgumentNull()
    {
      var pieceType = _board.PieceSet[PT.馬];
      var position = Position.Parse("1a");
      var player = _board.White;

      MyAssert.ThrowsException<ArgumentNullException>(
        () => _board.SetPiece(pieceType, position, null));

      MyAssert.ThrowsException<ArgumentNullException>(
        () => _board.SetPiece((Piece)null, position, player));

      MyAssert.ThrowsException<ArgumentNullException>(
        () => _board.SetPiece((IPieceType)null, position, player));

      MyAssert.ThrowsException<ArgumentNullException>(
        () => _board.SetPiece(null, position));
    }
    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void SetPieceWhichIsAlreadySet()
    {
      var piece = _board.PieceSet[PT.馬];
      _board.SetPiece(piece, _board.GetCellAt(0, 0).Position, _board.White);
      _board.SetPiece(piece, _board.GetCellAt(0, 1).Position, _board.White);
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
    [TestMethod]
    public void SetPieceTest()
    {
      _board.SetPiece(PT.馬, "5g", PieceColor.White);
      Assert.IsNotNull(_board.GetPieceAt("5g"));
    }
    [TestMethod, ExpectedException(typeof(NotEnoughPiecesInSetException))]
    public void SetNonexistingPiece()
    {
      _board.SetPiece(PT.馬, "1i", PieceColor.Black);
      _board.SetPiece(PT.馬, "2i", PieceColor.Black);
      _board.SetPiece(PT.馬, "3i", PieceColor.Black);
    }
    [TestMethod]
    public void SetWhitePiece()
    {
      _board.SetPiece(PT.馬, "1i", PieceColor.Black);

    }
  }
}