using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Moves;

namespace UnitTests.ShogiCore
{
  [TestClass]
  public class ShogiTest
  {
    private Board _board;

    [TestInitialize]
    public void Init()
    {
      _board = new Board();
    }


    [TestMethod]
    public void InitialPositionTest()
    {
      //    ______________
      //___/ Create board \__________________________________________________
      Shogi.InitBoard(_board);
      //    _________________________________
      //___/ Check pieces which must present \_______________________________
      foreach (var pair in Shogi.InitialPosition)
        Assert.AreEqual(pair.Value, (string)_board[pair.Key].Type);
      //    _________________________________
      //___/ Check cells which must be empty \_______________________________
      for (int i = 0; i < 9; i++)
        for (int j = 0; j < 9; j++)
          if (!Shogi.InitialPosition.ContainsKey(new Position(i, j)))
            Assert.IsNull(_board[i, j].Piece);
    }
    [TestMethod]
    public void CellBindabilityTest()
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

      Shogi.InitBoard(_board);
      Assert.AreEqual(counter, Shogi.InitialPosition.Count());
    }
  }
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
    public void SetPieceTest()
    {
      _board.SetPiece("5g", PieceColor.White, "馬");
      Assert.IsNotNull(_board["5g"]);
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
      Shogi.InitBoard(board);
      var move = board.GetUsualMove("3c", "3d", false);
      _board.MakeMove(move);
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void SetPlayerFromAnotherBoard()
    {
      _board.OneWhoMoves = new Board().White;
    }

    #region ' CurrentSnapshot Property '

    [TestMethod]
    public void ResetSnapshotOnPlayerChangeTest()
    {
      var snapshot1 = _board.CurrentSnapshot;
      _board.OneWhoMoves = _board.Black;
      var snapshot2 = _board.CurrentSnapshot;

      Assert.AreEqual(PieceColor.White, snapshot1.OneWhoMoves);
      Assert.AreEqual(PieceColor.Black, snapshot2.OneWhoMoves);
      Assert.AreNotSame(snapshot1, snapshot2);
    }
    [TestMethod]
    public void CurrentSnapshotBindabilityTest()
    {
      //    ____________
      //___/ First case \__________________________________________________
      var assertion1 = new PropertyObserverAssertion<Board>(_board).
        RegisterCounter(b => b.CurrentSnapshot, 0);

      // Here we never read _board.CurrentSnapshot property 
      // so there's nothing to change
      _board.SetPiece("1i", PieceColor.Black, "馬");
      _board.OneWhoMoves = _board.OneWhoMoves.Oppenent;

      assertion1.Check();

      //    _____________
      //___/ Second case \_________________________________________________
      var assertion2 = new PropertyObserverAssertion<Board>(_board).
        RegisterCounter(b => b.CurrentSnapshot, 2);

      // Here we read _board.CurrentSnapshot property every time 
      // so every time snapshot changes we get notification
      Assert.IsNotNull(_board.CurrentSnapshot);
      _board.SetPiece("1i", PieceColor.Black, "馬");
      Assert.IsNotNull(_board.CurrentSnapshot);
      _board.OneWhoMoves = _board.OneWhoMoves.Oppenent;

      assertion2.Check();
    }
    [TestMethod]
    public void CurrentsSnapshotHasMeaningfullDataTest()
    {
      Shogi.InitBoard(_board);
      var s = _board.CurrentSnapshot;
      var board = new Board();
      board.LoadSnapshot(s);

      foreach (var p in Position.OnBoard)
        if (board[p] != null)
        {
          Assert.AreEqual((string)board[p].Type, Shogi.InitialPosition[p]);
        }
        else
        {
          Assert.IsFalse(Shogi.InitialPosition.ContainsKey(p));
        }
    }

    #endregion

    #region ' IsMovesOrderMaintained Property '

    [TestMethod]
    public void IsMovesOrderMaintainedPropertyTest()
    {
      Shogi.InitBoard(_board);
      _board.IsMovesOrderMaintained = false;
      // Make move for black twice and check there's no exception
      _board.MakeMove(_board.GetUsualMove("3c", "3d", false));
      _board.MakeMove(_board.GetUsualMove("3d", "3e", false));
    }
    [TestMethod]
    public void IsMovesOrderMaintainedKeepsValueTest()
    {
      Assert.IsTrue(_board.IsMovesOrderMaintained);
      _board.IsMovesOrderMaintained = false;
      Assert.IsFalse(_board.IsMovesOrderMaintained);
      _board.IsMovesOrderMaintained = true;
      Assert.IsTrue(_board.IsMovesOrderMaintained);
    }
    [TestMethod]
    public void IsMovesOrderMaintainedBindabilityTest()
    {
      var assertion = new PropertyObserverAssertion<Board>(_board).
        RegisterCounter(b => b.IsMovesOrderMaintained, 2);

      _board.IsMovesOrderMaintained = false;
      _board.IsMovesOrderMaintained = false;
      _board.IsMovesOrderMaintained = true;

      assertion.Check();
    }

    #endregion

    #region ' Ctors '

    [TestMethod]
    public void NoArgsCtorTest()
    {
      var board = new Board();
      Assert.IsTrue(board.PieceSet is DefaultPieceSet);
      Assert.IsNotNull(board.White);
      Assert.IsNotNull(board.Black);
      Assert.IsNotNull(board.OneWhoMoves);
      foreach (var p in Position.OnBoard)
        Assert.IsNotNull(board[p.X, p.Y]);
    }
    [TestMethod]
    public void PieceSetTypeCtorTest()
    {
      var board = new Board(PieceSetType.Inifinite);
      Assert.IsTrue(board.PieceSet is InfinitePieceSet);
      Assert.IsNotNull(board.White);
      Assert.IsNotNull(board.Black);
      Assert.IsNotNull(board.OneWhoMoves);
      foreach (var p in Position.OnBoard)
        Assert.IsNotNull(board[p.X, p.Y]);
    }
    [TestMethod]
    public void InvalidPieceSetTypeCtorTest()
    {
      var board = new Board((PieceSetType)4);
      Assert.IsTrue(board.PieceSet is DefaultPieceSet);
      Assert.IsNotNull(board.White);
      Assert.IsNotNull(board.Black);
      Assert.IsNotNull(board.OneWhoMoves);
      foreach (var p in Position.OnBoard)
        Assert.IsNotNull(board[p.X, p.Y]);
    }

    #endregion

    [TestMethod]
    public void PlayerIndexerTest()
    {
      Assert.AreSame(_board.White, _board[PieceColor.White]);
      Assert.AreSame(_board.Black, _board[PieceColor.Black]);
    }
    [TestMethod]
    public void CellsPropertyTest()
    {
      Assert.AreEqual(81, _board.Cells.Count());
    }

    #region ' GetAvailableMoves Method '

    [TestMethod]
    public void GetAvailableUsualMovesTest()
    {
      Shogi.InitBoard(_board);
      var availableMoves = _board.GetAvailableMoves("4a");
      var toPositions = (from UsualMove m in availableMoves select m.To).ToList();
      CollectionAssert.AreEquivalent(new Position[] { "5b", "4b", "3b" }, toPositions);
    }

    [TestMethod]
    public void TestGetAvailableDropMovesByPiece()
    {
      var piece = _board.White.AddToHand("馬");
      var availableMoves = _board.GetAvailableMoves(piece);
      var toPositions = (from m in availableMoves select m.To).ToList();
      CollectionAssert.AreEquivalent(Position.OnBoard.ToList(), toPositions);
    }

    [TestMethod]
    public void TestGetAvailableDropMovesByPieceType()
    {
      _board.White.Hand.Add(_board.PieceSet["馬"]);
      // When get in hand 馬 turns into 角
      var availableMoves = _board.GetAvailableMoves("角", PieceColor.White);
      var toPositions = (from m in availableMoves select m.To).ToList();
      CollectionAssert.AreEquivalent(Position.OnBoard.ToList(), toPositions);
    }

    #endregion

    [TestMethod, ExpectedException(typeof(PieceHasNoOwnerException))]
    public void CantUseOwnerlessPieceInGetAvailableMovesTest()
    {
      _board.GetAvailableMoves(_board.PieceSet["馬"]);
    }
    [TestMethod, ExpectedException(typeof(PieceHasNoOwnerException))]
    public void CantUseOwnerlessPieceInGetDropMoveTest()
    {
      _board.GetDropMove(_board.PieceSet["馬"], "1a");
    }
  }
}