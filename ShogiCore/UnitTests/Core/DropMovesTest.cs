using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.PieceSets;
using Yasc.ShogiCore.Primitives;
using Yasc.ShogiCore.Snapshots;

namespace ShogiCore.UnitTests.Core
{
  [TestClass]
  public class DropMovesTest
  {
    private Board _board;
    private Piece _blackPiece;
    private Piece _whitePiece;

    [TestInitialize]
    public void Init()
    {
      _board = new Board(new StandardPieceSet());
      
      _board.LoadSnapshot(BoardSnapshot.InitialPosition);
      
      _blackPiece = _board.GetPieceAt("9a");
      _board.ResetPiece("9a");
      _board.Black.Hand.Add(_blackPiece);

      _whitePiece = _board.GetPieceAt("9i");
      _board.ResetPiece("9i");
      _board.White.Hand.Add(_whitePiece);
    }

    [TestMethod]
    public void SimplestTest()
    {
      var move = _board.GetDropMove(_blackPiece.PieceType, "9e", _board.Black);
      _board.MakeMove(move);
      Assert.AreSame(_blackPiece, _board.GetPieceAt("9e"));
      Assert.AreSame(_board.Black, _blackPiece.Owner);
    }
    [TestMethod]
    public void MovesOrderMaintenanceFalse()
    {
      _board.IsMovesOrderMaintained = false;
      var move = _board.GetDropMove(PT.歩, "9e", _board.OneWhoMoves);
      Assert.AreEqual(RulesViolation.WrongPieceReference, move.RulesViolation);
    }
    [TestMethod]
    public void MovesOrderMaintenanceTrue()
    {
      _board.OneWhoMoves = _board.White;
      var move = _board.GetDropMove(_blackPiece, "5e");
      Assert.AreEqual(RulesViolation.WrongSideToMove, move.RulesViolation);
    }
  
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void DropNullPiece()
    {
      _board.GetDropMove(null, "5g");
    }

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void TryDropPieceWhichIsOnTheBoard()
    {
      _board.GetDropMove(_board.GetPieceAt("1a"), "9e");
    }
  }
}