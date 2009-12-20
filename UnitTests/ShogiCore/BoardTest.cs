using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.Netwroking;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Moves;
using Yasc.ShogiCore.Snapshots;

namespace UnitTests.ShogiCore
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

    #region ' Set / Reset piece '

    [TestMethod]
    public void SetPieceTest()
    {
      _board.SetPiece("馬", PieceColor.White, "5g");
      Assert.IsNotNull(_board["5g"]);
    }
    [TestMethod, ExpectedException(typeof(NotEnoughPiecesInSetException))]
    public void CantSetPieceBecauseNotEnoughPiecesTest()
    {
      _board.SetPiece("馬", PieceColor.Black, "1i");
      _board.SetPiece("馬", PieceColor.Black, "2i");
      _board.SetPiece("馬", PieceColor.Black, "3i");
    }
    [TestMethod]
    public void SetWhitePiece()
    {
      _board.SetPiece("馬", PieceColor.Black, "1i");

    }
    #endregion

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void MakeNullMoveTest()
    {
      _board.MakeMove(null);
    }
    [TestMethod, ExpectedException(typeof(InvalidMoveException))]
    public void MakeInvalidMoveTest()
    {
      _board.MakeMove(_board.GetMove("1i-1i"));
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
      _board.SetPiece("馬", PieceColor.Black, "1i");
      _board.OneWhoMoves = _board.OneWhoMoves.Opponent;

      assertion1.Check();

      //    _____________
      //___/ Second case \_________________________________________________
      var assertion2 = new PropertyObserverAssertion<Board>(_board).
        RegisterCounter(b => b.CurrentSnapshot, 2);

      // Here we read _board.CurrentSnapshot property every time 
      // so every time snapshot changes we get notification
      Assert.IsNotNull(_board.CurrentSnapshot);
      _board.SetPiece("馬", PieceColor.Black, "1i");
      Assert.IsNotNull(_board.CurrentSnapshot);
      _board.OneWhoMoves = _board.OneWhoMoves.Opponent;

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
          Assert.AreEqual((string)board[p].PieceType, Shogi.InitialPosition[p]);
        }
        else
        {
          Assert.IsFalse(Shogi.InitialPosition.ContainsKey(p));
        }
    }

    #endregion

    #region ' IsMovesOrderMaintained Property '

    [TestMethod]
    public void IsMovesOrderMaintainedForUsualMovesTest()
    {
      Shogi.InitBoard(_board);
      _board.IsMovesOrderMaintained = false;
      // Make move for black twice and check there's no exception
      _board.MakeMove(_board.GetUsualMove("3c", "3d", false));
      _board.MakeMove(_board.GetUsualMove("3d", "3e", false));
    }
    [TestMethod]
    public void IsMovesOrderMaintainedForDropMovesTest()
    {
      var piece1 = _board.Black.AddToHand("馬");
      _board.Black.AddToHand("馬");
      _board.IsMovesOrderMaintained = false;
      // Make move for black twice and check there's no exception
      _board.MakeMove(_board.GetDropMove(piece1, "1i"));
      _board.MakeMove(_board.GetDropMove("角", "2i", _board.Black));
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

    #region ' Ctors / Snapshot loading '

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
      var board = new Board(PieceSetType.Infinite);
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
    [TestMethod, ExpectedException(typeof(NotEnoughPiecesInSetException))]
    public void CantLoadSnapshotBecauseNotEnoughPiecesTest()
    {
      var board = new Board(PieceSetType.Infinite);
      board.SetPiece("馬", PieceColor.Black, "1i");
      board.SetPiece("馬", PieceColor.Black, "2i");
      board.SetPiece("馬", PieceColor.Black, "3i");

      _board.LoadSnapshot(board.CurrentSnapshot);
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

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void PassNullGetAvailableUsualMovesTest()
    {
      _board.GetAvailableMoves((Piece)null);
    }
    [TestMethod]
    public void GetAvailableUsualMovesTest()
    {
      Shogi.InitBoard(_board);
      var availableMoves = _board.GetAvailableMoves("4a");
      var toPositions = (from UsualMove m in availableMoves select m.To).ToList();
      CollectionAssert.AreEquivalent(new Position[] { "5b", "4b", "3b" }, toPositions);
    }
    [TestMethod]
    public void GetAvailableMovesOrderTest()
    {
      _board.IsMovesOrderMaintained = false;
      Shogi.InitBoard(_board);
      var availableMoves = _board.GetAvailableMoves("7g");
      var toPositions = (from UsualMove m in availableMoves select m.To).ToList();
      CollectionAssert.AreEquivalent(new Position[] { "7f" }, toPositions);
      Assert.AreEqual(_board.Black, _board.OneWhoMoves);
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
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetDropMoveNullPieceArgTest()
    {
      _board.GetDropMove(null, "1i");
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetDropMoveAlienPieceArgTest()
    {
      _board.GetDropMove(new Board().White.AddToHand("歩"), "1i");
    }
    
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void PassWrongPlayerToGetDropMoveTest()
    {
      _board.GetDropMove("馬", "1i", new Board().White);
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void PassWrongPlayerToSetPieceA()
    {
      _board.SetPiece(_board.PieceSet["馬"], new Board().White, "1i");
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void PassWrongPlayerToSetPieceB()
    {
      _board.SetPiece("馬", new Board().White, "1i");
    }

    #region ' OnMoving/OnMoved Events '

    [TestMethod]
    public void OnMoveEventTest()
    {
      var log = new TestLog();
      _board.Moved += (s, e) => log.Write(string.Format("Moved({0})", e.Move));
      _board.Moving += (s, e) => log.Write(string.Format("Moving({0})", e.Move));
      Shogi.InitBoard(_board);
      _board.MakeMove(_board.GetMove("1c-1d"));
      Assert.AreEqual("Moving(1c-1d) Moved(1c-1d)", log.ToString());
    }

    #endregion

    #region ' Hostory Navigation '
    
    [TestMethod]
    public void HistoryNavigationEventsTest()
    {
      var log = new TestLog();
      _board.HistoryNavigating += (s, e) =>
        log.Write(string.Format("HistoryNavigating({0})", e.Step));
      _board.HistoryNavigated += (s, e) => 
        log.Write(string.Format("HistoryNavigated({0})", e.Step));
      Shogi.InitBoard(_board);
      _board.MakeMove(_board.GetMove("1c-1d"));
      _board.MakeMove(_board.GetMove("1g-1f"));
      _board.MakeMove(_board.GetMove("2c-2d"));
      _board.History.CurrentMoveIndex = 0;
      Assert.AreEqual("HistoryNavigating(-2) HistoryNavigated(-2)", 
        log.ToString());
    }

    [TestMethod]
    public void HistoryNavigationSnapshotsTest()
    {
      Shogi.InitBoard(_board);
      var snapshot = new BoardSnapshot[1];

      _board.HistoryNavigating += (s, e) =>
                                    {
                                      Assert.AreNotEqual(snapshot[0], _board.CurrentSnapshot);
                                      Assert.AreEqual(snapshot[0], e.Snapshot);
                                    };
      _board.HistoryNavigated += (s, e) =>
                                   {
                                     Assert.AreEqual(snapshot[0], _board.CurrentSnapshot);
                                     Assert.AreEqual(snapshot[0], e.Snapshot);
                                   };

      _board.MakeMove(_board.GetMove("1c-1d"));
      snapshot[0] = _board.CurrentSnapshot;
      _board.MakeMove(_board.GetMove("1g-1f"));
      _board.MakeMove(_board.GetMove("2c-2d"));
      _board.History.CurrentMoveIndex = 0;
    }

    #endregion

  }
}