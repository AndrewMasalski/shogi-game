using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.Notations;
using Yasc.ShogiCore.PieceSets;
using Yasc.ShogiCore.Primitives;
using Yasc.ShogiCore.Snapshots;
using Yasc.Utils;

namespace ShogiCore.UnitTests.Core
{
  [TestClass]
  public class BoardTest
  {
    private Board _board;

    [TestInitialize]
    public void Init()
    {
      _board = new Board(new StandardPieceSet());
    }

    [TestMethod]
    public void MakeMove()
    {
      MyAssert.ThrowsException<ArgumentNullException>(
        () => _board.MakeMove(null));
      
      MyAssert.ThrowsException<InvalidMoveException>(
        () => _board.MakeMove(_board.GetUsualMove("1i", "1i")));

      MyAssert.ThrowsException<ArgumentOutOfRangeException>(() =>
        {
          var anotherBoard = new Board(new StandardPieceSet());
          anotherBoard.LoadSnapshot(BoardSnapshot.InitialPosition);
          _board.MakeMove(anotherBoard.GetUsualMove("3c", "3d"));
        });
    }

    [TestMethod]
    public void OneWhoMoves()
    {
      MyAssert.ThrowsException<ArgumentOutOfRangeException>(() => {
        _board.OneWhoMoves = new Board(new StandardPieceSet()).White; });
    }

    #region ' CurrentSnapshot Property '

    [TestMethod]
    public void EventsWhichResetCurrentSnapshot()
    {
      var snapshot = _board.CurrentSnapshot;
      var check = new Action(() =>
                               {
                                 var s = _board.CurrentSnapshot;
                                 Assert.AreNotSame(snapshot, s);
                                 snapshot = s;
                               });
      
      _board.OneWhoMoves = _board.White;
      check();
      _board.White.Hand.Add(PT.香);
      check();
      _board.Black.Hand.Add(PT.香);
      check();
      _board.SetPiece(PT.香, "1a", PieceColor.Black);
      check();
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
      _board.SetPiece(PT.馬, "1i", PieceColor.Black);
      _board.OneWhoMoves = _board.OneWhoMoves.Opponent;

      assertion1.Check();

      //    _____________
      //___/ Second case \_________________________________________________
      var assertion2 = new PropertyObserverAssertion<Board>(_board).
        RegisterCounter(b => b.CurrentSnapshot, 2);

      // Here we read _board.CurrentSnapshot property every time 
      // so every time snapshot changes we get notification
      Assert.IsNotNull(_board.CurrentSnapshot);
      _board.SetPiece(PT.馬, "1i", PieceColor.Black);
      Assert.IsNotNull(_board.CurrentSnapshot);
      _board.OneWhoMoves = _board.OneWhoMoves.Opponent;

      assertion2.Check();
    }
    [TestMethod]
    public void CurrentSnapshotIsValid()
    {
      _board.White.Hand.Add(PT.香);
      _board.Black.Hand.Add(PT.歩);
      _board.Black.Hand.Add(PT.歩);
      _board.SetPiece(PT.玉, "1a", PieceColor.White);
      _board.SetPiece(PT.王, "1i", PieceColor.Black);

      var snapshot = _board.CurrentSnapshot;
      CollectionAssert.AreEquivalent(new[] { PT.香 }, snapshot.WhiteHand.ToList());
      CollectionAssert.AreEquivalent(new[] { PT.歩, PT.歩 }, snapshot.BlackHand.ToList());
      Assert.AreEqual(2, snapshot.Cells.Count(c => c != null));
      Assert.AreEqual(PieceColor.White, snapshot.GetPieceAt("1a").Color);
      Assert.AreEqual(PieceColor.Black, snapshot.GetPieceAt("1i").Color);
      Assert.AreEqual(PT.玉, snapshot.GetPieceAt("1a").PieceType);
      Assert.AreEqual(PT.王, snapshot.GetPieceAt("1i").PieceType);
    }

    #endregion

    #region ' IsMovesOrderMaintained Property '

    [TestMethod]
    public void IsMovesOrderMaintainedForUsualMovesTest()
    {
      _board.LoadSnapshot(BoardSnapshot.InitialPosition);
      _board.IsMovesOrderMaintained = false;
      // Make move for black twice and check there's no exception
      _board.MakeMove(_board.GetUsualMove("3c", "3d"));
      _board.MakeMove(_board.GetUsualMove("3d", "3e"));
    }
    [TestMethod]
    public void IsMovesOrderMaintainedForDropMovesTest()
    {
      var piece1 = _board.Black.Hand.Add(PT.馬);
      _board.Black.Hand.Add(PT.馬);
      _board.IsMovesOrderMaintained = false;
      // Make move for black twice and check there's no exception
      _board.MakeMove(_board.GetDropMove(piece1, "1i"));
      _board.MakeMove(_board.GetDropMove(PT.角, "2i", _board.Black));
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
      var board = new Board(new StandardPieceSet());
      Assert.IsTrue(board.PieceSet is StandardPieceSet);
      Assert.IsNotNull(board.White);
      Assert.IsNotNull(board.Black);
      Assert.IsNotNull(board.OneWhoMoves);
      foreach (var p in Position.OnBoard)
        Assert.IsNotNull(board.GetCellAt(p));
    }
    [TestMethod]
    public void PieceSetTypeCtorTest()
    {
      var board = new Board(InfinitePieceSet.Instance);
      Assert.IsTrue(board.PieceSet is InfinitePieceSet);
      Assert.IsNotNull(board.White);
      Assert.IsNotNull(board.Black);
      Assert.IsNotNull(board.OneWhoMoves);
      foreach (var p in Position.OnBoard)
        Assert.IsNotNull(board.GetCellAt(p));
    }
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void InvalidPieceSetTypeCtorTest()
    {
      new Board(null);
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

    #endregion

    [TestMethod]
    public void PlayerIndexerTest()
    {
      Assert.AreSame(_board.White, _board.GetPlayer(PieceColor.White));
      Assert.AreSame(_board.Black, _board.GetPlayer(PieceColor.Black));
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
      _board.GetAvailableMoves(null);
    }
    [TestMethod]
    public void GetAvailableUsualMovesTest()
    {
      _board.LoadSnapshot(BoardSnapshot.InitialPosition);
      var availableMoves = _board.GetAvailableMoves("4a");
      var toPositions = (from UsualMove m in availableMoves select m.To).ToList();
      CollectionAssert.AreEquivalent(P("5b", "4b", "3b"), toPositions);
    }
    private static List<Position>P(params string[] positions)
    {
      return positions.Select(Position.Parse).ToList();
    }
    [TestMethod]
    public void GetAvailableMovesOrderTest()
    {
      _board.IsMovesOrderMaintained = false;
      _board.LoadSnapshot(BoardSnapshot.InitialPosition);
      var availableMoves = _board.GetAvailableMoves("7g");
      var toPositions = (from UsualMove m in availableMoves select m.To).ToList();
      CollectionAssert.AreEquivalent(P("7f"), toPositions);
      Assert.AreEqual(_board.Black, _board.OneWhoMoves);
    }

    [TestMethod]
    public void TestGetAvailableDropMovesByPiece()
    {
      var piece = _board.Black.Hand.Add(PT.馬);
      var availableMoves = _board.GetAvailableMoves(piece);
      var toPositions = (from m in availableMoves select m.To).ToList();
      CollectionAssert.AreEquivalent(Position.OnBoard.ToList(), toPositions);
    }

    [TestMethod]
    public void TestGetAvailableDropMovesByPieceType()
    {
      _board.Black.Hand.Add(_board.PieceSet[PT.馬]);
      // When got in hand 馬 turns into 角
      var availableMoves = _board.GetAvailableMoves(PT.角, PieceColor.Black);
      var toPositions = (from m in availableMoves select m.To).ToList();
      CollectionAssert.AreEquivalent(Position.OnBoard.ToList(), toPositions);
    }
    [TestMethod, ExpectedException(typeof(PieceNotFoundException))]
    public void TestGetAvailableDropMovesByPieceForUnexistingPieceType()
    {
      _board.GetAvailableMoves(PT.角, PieceColor.Black);
    }

    #endregion

    [TestMethod, ExpectedException(typeof(PieceHasNoOwnerException))]
    public void CantUseOwnerlessPieceInGetAvailableMovesTest()
    {
      _board.GetAvailableMoves(_board.PieceSet[PT.馬]);
    }
    [TestMethod, ExpectedException(typeof(PieceHasNoOwnerException))]
    public void CantUseOwnerlessPieceInGetDropMoveTest()
    {
      _board.GetDropMove(_board.PieceSet[PT.馬], "1a");
    }
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetDropMoveNullPieceArgTest()
    {
      _board.GetDropMove(null, "1i");
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetDropMoveAlienPieceArgTest()
    {
      _board.GetDropMove(new Board(new StandardPieceSet()).White.Hand.Add(PT.歩), "1i");
    }
    
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void PassWrongPlayerToGetDropMoveTest()
    {
      _board.GetDropMove(PT.馬, "1i", new Board(new StandardPieceSet()).White);
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void PassWrongPlayerToSetPieceA()
    {
      _board.SetPiece(_board.PieceSet[PT.馬], "1i", new Board(new StandardPieceSet()).White);
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void PassWrongPlayerToSetPieceB()
    {
      _board.SetPiece(PT.馬, "1i", new Board(new StandardPieceSet()).White);
    }

    #region ' OnMoving/OnMoved Events '

    [TestMethod]
    public void OnMoveEventTest()
    {
      var log = new TestLog();
      _board.Moved += (s, e) => log.Write(string.Format("Moved({0})", e.Move));
      _board.Moving += (s, e) => log.Write(string.Format("Moving({0})", e.Move));
      _board.LoadSnapshot(BoardSnapshot.InitialPosition);
      _board.MakeMove(_board.GetMove("1g-1f", FormalNotation.Instance).First());
      Assert.AreEqual("Moving(1g-1f) Moved(1g-1f)", log.ToString());
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
      _board.LoadSnapshot(BoardSnapshot.InitialPosition);
      _board.MakeMove(_board.GetMove("1g-1f", FormalNotation.Instance).First());
      _board.MakeMove(_board.GetMove("1c-1d", FormalNotation.Instance).First());
      _board.MakeMove(_board.GetMove("2g-2f", FormalNotation.Instance).First());
      _board.History.CurrentMoveIndex = 0;
      Assert.AreEqual("HistoryNavigating(-2) HistoryNavigated(-2)", 
        log.ToString());
    }

    [TestMethod]
    public void HistoryNavigationSnapshotsTest()
    {
      _board.LoadSnapshot(BoardSnapshot.InitialPosition);
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

      _board.MakeMove(_board.GetMove("1g-1f", FormalNotation.Instance).First());
      snapshot[0] = _board.CurrentSnapshot;
      _board.MakeMove(_board.GetMove("1c-1d", FormalNotation.Instance).First());
      _board.MakeMove(_board.GetMove("2g-2f", FormalNotation.Instance).First());
      _board.History.CurrentMoveIndex = 0;
    }

    #endregion

    [TestMethod]
    public void ParseCuteMoveTest()
    {
      _board.LoadSnapshot(BoardSnapshot.InitialPosition);
      var move = _board.GetMove("P2f", CuteNotation.Instance).Cast<UsualMove>().First();
      Assert.AreEqual("2g", move.From.ToString());
      Assert.AreEqual("2f", move.To.ToString());
      Assert.IsFalse(move.IsPromoting);
    }
    [TestMethod]
    public void ParseFormalMoveTest()
    {
      _board.LoadSnapshot(BoardSnapshot.InitialPosition);
      var move = _board.GetMove("2g-2f", FormalNotation.Instance).Cast<UsualMove>().First();
      Assert.AreEqual("2g", move.From.ToString());
      Assert.AreEqual("2f", move.To.ToString());
      Assert.IsFalse(move.IsPromoting);
    }

    [TestMethod]
    public void TestBlackResignMove()
    {
      Assert.AreEqual(ShogiGameResult.None, _board.GameResult);
      _board.MakeMove(_board.GetResignMove());
      Assert.AreEqual(ShogiGameResult.WhiteWin, _board.GameResult);
    }

    [TestMethod]
    public void TestWhiteResignMove()
    {
      _board.OneWhoMoves = _board.GetPlayer(PieceColor.White);
      Assert.AreEqual(ShogiGameResult.None, _board.GameResult);
      _board.MakeMove(_board.GetResignMove());
      Assert.AreEqual(ShogiGameResult.BlackWin, _board.GameResult);
    }
  }
}