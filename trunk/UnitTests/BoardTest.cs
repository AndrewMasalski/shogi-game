using System;
using System.Collections;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Moves;
using Yasc.ShogiCore.Utils;

namespace UnitTests
{
  [TestClass]
  public class BoardTest
  {
    private Board _board;

    [TestInitialize]
    public void Init()
    {
      _board = new Board();
    }
    [TestMethod]
    public void TestInitialPosition()
    {
      //    ______________
      //___/ Create board \__________________________________________________
      Shogi.InititBoard(_board);
      //    _________________________________
      //___/ Check pieces which must present \_______________________________
      foreach (var pair in Shogi.InitialPosition)
        Assert.AreEqual(pair.Value, (string) _board[pair.Key].Type);
      //    _________________________________
      //___/ Check cells which must be empty \_______________________________
      for (int i = 0; i < 9; i++)
        for (int j = 0; j < 9; j++)
          if (!Shogi.InitialPosition.ContainsKey(new Position(i, j)))
            Assert.IsNull(_board[i, j].Piece);
    }
    [TestMethod]
    public void CheckPlaceHoldersBindability()
    {
      int counter = 0;
      var handler = new Action<Position, Cell>(
        (cell, placeHolder) =>
          {
            counter++;
            Assert.AreEqual(Shogi.InitialPosition[cell], (string)placeHolder.Piece.Type);
          });

      for (int i = 0; i < 9; i++)
        for (int j = 0; j < 9; j++)
        {
          int x = i;
          int y = j;
          _board[i, j].PropertyChanged += 
            (s, e) => handler(new Position(x, y), (Cell)s);
        }

      Shogi.InititBoard(_board);
      Assert.AreEqual(counter, Shogi.InitialPosition.Count());
    }
    [TestMethod]
    public void PutPieceTest()
    {
      _board["5g"] = new Piece(_board.White, "馬");
      Assert.IsNotNull(_board["5g"]);
    }
    [TestMethod]
    public void ValidMoveWithoutTakingPieceTest()
    {
      Shogi.InititBoard(_board);
      var move = _board.GetUsualMove("9c", "9d", false);
      Assert.IsNotNull(move);
      _board.MakeMove(move);
      Assert.IsNull(_board["9c"]);
      Assert.AreEqual(PieceType.歩, _board["9d"].Type);
    }
    [TestMethod]
    public void ValidMoveWithTakingPieceTest()
    {
      Shogi.InititBoard(_board);

      _board.MakeMove(_board.GetUsualMove("9c", "9d", false)); _board.OneWhoMoves = _board.White;
      _board.MakeMove(_board.GetUsualMove("9d", "9e", false)); _board.OneWhoMoves = _board.White;
      _board.MakeMove(_board.GetUsualMove("9e", "9f", false)); _board.OneWhoMoves = _board.White;
      _board.MakeMove(_board.GetUsualMove("9f", "9g", false));

      Assert.IsNull(_board["9c"]);
      Assert.IsNull(_board["9d"]);
      Assert.IsNull(_board["9e"]);
      Assert.IsNull(_board["9f"]);

      Assert.AreEqual(PieceType.歩, _board["9g"].Type);
      Assert.AreEqual(_board.White, _board["9g"].Owner);
      Assert.AreEqual(1, _board.White.Hand.Count);
      Assert.AreEqual(PieceType.歩, _board.White.Hand[0].Type);
      Assert.AreEqual(_board.White, _board.White.Hand[0].Owner);
    }
    [TestMethod, ExpectedException(typeof(InvalidMoveException))]
    public void InvalidMoveTest()
    {
      var badMove = _board.GetUsualMove("1c", "1d", false);
      _board.MakeMove(badMove);                 
    }
    [TestMethod]
    public void InvalidMoveMessageTest()
    {
      var badMove = _board.GetUsualMove("1c", "1d", false);
      Assert.IsFalse(badMove.IsValid);
      Assert.AreEqual("No piece at 1c", badMove.ErrorMessage);
    }
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void MakeNullMoveTest()
    {
      _board.MakeMove(null);
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void MakeMoveFromAnotherBoard()
    {
      var board = new Board();
      Shogi.InititBoard(board);
      var move = board.GetUsualMove("3c", "3d", false);
      _board.MakeMove(move);
    }
    [TestMethod, ExpectedException(typeof(InvalidMoveException))]
    public void GetUsualMoveFromEmptyCell()
    {
      var move = _board.GetUsualMove("3d", "3e", false);

      Assert.IsFalse(move.IsValid);
      Assert.AreEqual("No piece at 3d", move.ErrorMessage);
      _board.MakeMove(move);
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void SetPlayerFromAnotherBoard()
    {
      _board.OneWhoMoves = new Board().White;
    }
    [TestMethod]
    public void ResetSnapshotOnPlayerChange()
    {
      Assert.AreEqual(PieceColor.White, _board.CurrentSnapshot.OneWhoMoves);
      _board.OneWhoMoves = _board.Black;
      Assert.AreEqual(PieceColor.Black, _board.CurrentSnapshot.OneWhoMoves);
    }
    [TestMethod]
    public void PlayerIndexer()
    {
      Assert.AreSame(_board.White, _board[PieceColor.White]);
      Assert.AreSame(_board.Black, _board[PieceColor.Black]);
    }
    [TestMethod]
    public void TestEnumerable()
    {
      Assert.AreEqual(81, _board.Count());

      int counter = 0;
      for (var i = ((IEnumerable)_board).GetEnumerator(); i.MoveNext(); )
        counter++;

      Assert.AreEqual(81, counter);
    }
    [TestMethod]
    public void TestMoveOrder()
    {
      Shogi.InititBoard(_board);
      _board.MakeMove(_board.GetUsualMove("3c", "3d", false));
      Assert.AreEqual("It's Black's move now",
        _board.GetUsualMove("3d", "3e", false).ErrorMessage);
    }
    [TestMethod]
    public void TestIgnoreMoveOrder()
    {
      Shogi.InititBoard(_board);
      _board.IsMovesOrderMaintained = false;
      _board.MakeMove(_board.GetUsualMove("3c", "3d", false));
      _board.MakeMove(_board.GetUsualMove("3d", "3e", false));
    }
    [TestMethod]
    public void IsMovesOrderMaintainedNotificationTest()
    {
      int counter = 0;
      _board.PropertyChanged += (s, e) =>
        {
          counter++;
          Assert.AreEqual("IsMovesOrderMaintained", e.PropertyName);
        };
      _board.IsMovesOrderMaintained = false;
      _board.IsMovesOrderMaintained = false;
      Assert.AreEqual(1, counter);
      Assert.IsFalse(_board.IsMovesOrderMaintained);
    }
   
    [TestMethod]
    public void TestGetAvailableMoves()
    {
      Shogi.InititBoard(_board);
      var a = (from UsualMove m in _board.GetAvailableMoves("4a") 
              select m.To).ToList();
      CollectionAssert.AreEquivalent(new Position[]{"5b", "4b", "3b"}, a);
    }
  }
}