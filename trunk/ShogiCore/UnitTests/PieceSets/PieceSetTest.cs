using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.PieceSets;
using Yasc.ShogiCore.Primitives;

namespace ShogiCore.UnitTests.PieceSets
{
  [TestClass]
  public class PieceSetTest
  {
    private Board _board;
    private IPieceSet[] _sets;

    [TestInitialize]
    public void Init()
    {
      _board = new Board(new StandardPieceSet());
      _sets = new IPieceSet[] {InfinitePieceSet.Instance, new StandardPieceSet()};
    }

    [TestMethod]
    public void ResetOwnerOnRelease()
    {
      foreach (var set in _sets)
        ResetOwnerOnRelease(set);
    }
    private void ResetOwnerOnRelease(IPieceSet pieceSet)
    {
      var piece = pieceSet[PT.馬];
      pieceSet.AcquirePiece(piece);
      piece.Owner = _board.White;
      pieceSet.ReleasePiece(piece);
      Assert.IsNull(piece.Owner);
    }

    [TestMethod]
    public void TestTakeTwice()
    {
      foreach (var set in _sets)
        TestTakeTwice(set);
    }
    private static void TestTakeTwice(IPieceSet pieceSet)
    {
      var piece = pieceSet[PT.馬];
      pieceSet.AcquirePiece(piece);
      MyAssert.ThrowsException<InvalidOperationException>(
        () => pieceSet.AcquirePiece(piece));
    }

    [TestMethod]
    public void TestPromoted()
    {
      foreach (var set in _sets)
        TestPromoted(set);
    }
    private static void TestPromoted(IPieceSet pieceSet)
    {
      var piece = pieceSet[PT.馬];
      Assert.AreEqual(PT.馬, piece.PieceType);
    }

    [TestMethod]
    public void TestDuplicates()
    {
      foreach (var pieceType in PT.AllPieceTypes)
        foreach (var set in _sets)
          TestDuplicates(set, pieceType);
    }
    private static void TestDuplicates(IPieceSet pieceSet, IPieceType pieceType)
    {
      var set = new HashSet<Piece>();
      for (var i = 0; i < 100; i++)
      {
        var p = pieceSet[pieceType];
        if (p == null) break;
        pieceSet.AcquirePiece(p);
        Assert.IsTrue(set.Add(p), "Not all pieces are different objects");
      }
    }

    [TestMethod]
    public void NullArguments()
    {
      foreach (var set in _sets)
        NullArguments(set);
    }
    private static void NullArguments(IPieceSet pieceSet)
    {
      MyAssert.ThrowsException<ArgumentNullException>(
        () => pieceSet.AcquirePiece(null));

      MyAssert.ThrowsException<ArgumentNullException>(
        () => pieceSet.ReleasePiece(null));
    }

    [TestMethod]
    public void ReturnPieceTwice()
    {
      foreach (var set in _sets)
        ReturnPieceTwice(set);
    }
    private static void ReturnPieceTwice(IPieceSet pieceSet)
    {
      MyAssert.ThrowsException<InvalidOperationException>(
        () => pieceSet.ReleasePiece(pieceSet[PT.馬]));
    }
  }
}