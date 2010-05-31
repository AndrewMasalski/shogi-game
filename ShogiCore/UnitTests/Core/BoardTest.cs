using System;
using System.Collections.Generic;
using System.Linq;
using CommonUtils.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.Moves;
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
    private Board _initializedBoard;
    private Piece _blackPiece;
    private Piece _whitePiece;

    [TestInitialize]
    public void Init()
    {
      _board = new Board(new StandardPieceSet());

      _initializedBoard = new Board(new StandardPieceSet());
      _initializedBoard.LoadSnapshot(BoardSnapshot.InitialPosition);

      _blackPiece = _initializedBoard.GetPieceAt("9a");
      _initializedBoard.ResetPiece("9a");
      _initializedBoard.Black.Hand.Add(_blackPiece);

      _whitePiece = _initializedBoard.GetPieceAt("9i");
      _initializedBoard.ResetPiece("9i");
      _initializedBoard.White.Hand.Add(_whitePiece);
    }

    #region ' Ctor '

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

    #endregion

    // Properties: 

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
      // Make move for black twice; there's no exception
      _board.MakeMove(_board.GetUsualMove("3c", "3d"));
      _board.MakeMove(_board.GetUsualMove("3d", "3e"));
    }
    [TestMethod]
    public void IsMovesOrderMaintainedForDropMovesTest()
    {
      _board.IsMovesOrderMaintained = false;
      // Make move for black twice; there's no exception
      _board.MakeMove(_board.GetDropMove(_board.Black.Hand.Add(PT.馬), "1i"));
      _board.MakeMove(_board.GetDropMove(_board.Black.Hand.Add(PT.馬), "2i"));
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
    public void IsMovesOrderMaintainedForDropMovesWithEmptyHand()
    {
      _board.IsMovesOrderMaintained = false;
      var move = _board.GetDropMove(PT.歩, "9e", _board.OneWhoMoves);
      Assert.AreEqual(RulesViolation.WrongPieceReference, move.RulesViolation);
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

    // Methods: 

    #region ' SetPiece() '

    [TestMethod]
    public void SetPieceTest()
    {
      _board.SetPiece(PT.馬, "5g", PieceColor.White);
      Assert.IsNotNull(_board.GetPieceAt("5g"));
    }
    [TestMethod]
    public void SetPieceWrongPlayer()
    {
      var piece = _board.PieceSet[PT.馬];
      var wrongPlayer = new Board(new StandardPieceSet()).White;

      MyAssert.ThrowsException<ArgumentOutOfRangeException>(
        () => _board.SetPiece(piece, "1i", wrongPlayer));

      MyAssert.ThrowsException<ArgumentOutOfRangeException>(
        () => _board.SetPiece(PT.馬, "1i", wrongPlayer));
    }
    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void SetPieceTwice()
    {
      var piece = _board.PieceSet[PT.馬];
      _board.SetPiece(piece, "1i", PieceColor.White);
      _board.White.Hand.Add(piece);
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
    public void SetPieceOwnerless()
    {
      var piece = _board.PieceSet[PT.馬];
      _board.SetPiece(piece, _board.GetCellAt(0, 0).Position);
    }
    [TestMethod, ExpectedException(typeof(NotEnoughPiecesInSetException))]
    public void SetNonexistingPiece()
    {
      _board.SetPiece(PT.馬, "1i", PieceColor.Black);
      _board.SetPiece(PT.馬, "2i", PieceColor.Black);
      _board.SetPiece(PT.馬, "3i", PieceColor.Black);
    }

    #endregion

    #region ' MakeMove() '

    [TestMethod]
    public void MakeMoveInvalidArguments()
    {
      MyAssert.ThrowsException<ArgumentNullException>(
        () => _board.MakeMove((Move)null));

      MyAssert.ThrowsException<ArgumentNullException>(
        () => _board.MakeMove((DecoratedMove)null));

      MyAssert.ThrowsException<InvalidMoveException>(
        () => _board.MakeMove(_board.GetUsualMove("1i", "1i")));
    }
    [TestMethod]
    public void MakeMoveDrop()
    {
      var move = _initializedBoard.GetDropMove(
        _blackPiece.PieceType, "9e", _initializedBoard.Black);
      _initializedBoard.MakeMove(move);
      Assert.AreSame(_blackPiece, _initializedBoard.GetPieceAt("9e"));
      Assert.AreSame(_initializedBoard.Black, _blackPiece.Owner);
    }
    [TestMethod]
    public void MakeMoveUsual()
    {
      var b = _initializedBoard;
      b.IsMovesOrderMaintained = false;

      b.MakeMove(b.GetUsualMove("9c", "9d"));
      b.MakeMove(b.GetUsualMove("9d", "9e"));
      b.MakeMove(b.GetUsualMove("9e", "9f"));
      b.MakeMove(b.GetUsualMove("9f", "9g"));

      Assert.IsNull(b.GetPieceAt("9c"));
      Assert.IsNull(b.GetPieceAt("9d"));
      Assert.IsNull(b.GetPieceAt("9e"));
      Assert.IsNull(b.GetPieceAt("9f"));

      Assert.AreEqual(PT.歩, b.GetPieceAt("9g").PieceType);
      Assert.AreEqual(b.White, b.GetPieceAt("9g").Owner);
      Assert.AreEqual(2, b.White.Hand.Count);
      Assert.AreEqual(PT.歩, b.White.Hand[1].PieceType);
    }

    #endregion

    #region ' GetDropMove() '

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetDropMoveNull()
    {
      _board.GetDropMove(null, "5g");
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetDropMoveNoPieceInHand()
    {
      _initializedBoard.GetDropMove(_initializedBoard.GetPieceAt("1a"), "9e");
    }

    #endregion

    #region ' LoadSnapshot() '

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

    #region ' GetAvailableMoves() '

    [TestMethod]
    public void ArgumentNull()
    {
      MyAssert.ThrowsException<ArgumentNullException>(
        () => _board.GetAvailableMoves(null));

      MyAssert.ThrowsException<PieceNotFoundException>(
        () => _board.GetAvailableMoves(PT.角, PieceColor.Black));

      MyAssert.ThrowsException<ArgumentNullException>(
        () => _board.GetAvailableMoves(null, PieceColor.Black));

      MyAssert.ThrowsException<PieceHasNoOwnerException>(
        () => _board.GetAvailableMoves(_board.PieceSet[PT.馬]));
    }

    [TestMethod]
    public void GetAvailableUsualMovesTest()
    {
      _board.LoadSnapshot(BoardSnapshot.InitialPosition);
      var availableMoves = _board.GetAvailableMoves("4a");
      var toPositions = (from UsualMove m in availableMoves select m.To).ToList();
      CollectionAssert.AreEquivalent(P("5b", "4b", "3b"), toPositions);
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

    #endregion

    #region ' GetResignMove() '

    [TestMethod]
    public void TestBlackResignMove()
    {
      Assert.AreEqual(ShogiGameState.NotDefined, _board.GameState);
      _board.MakeMove(_board.GetResignMove());
      Assert.AreEqual(ShogiGameState.WhiteWin, _board.GameState);
    }

    [TestMethod]
    public void TestWhiteResignMove()
    {
      _board.OneWhoMoves = _board.GetPlayer(PieceColor.White);
      Assert.AreEqual(ShogiGameState.NotDefined, _board.GameState);
      _board.MakeMove(_board.GetResignMove());
      Assert.AreEqual(ShogiGameState.BlackWin, _board.GameState);
    }

    #endregion

    [TestMethod]
    public void GetDropMoveWrongArgs()
    {
      MyAssert.ThrowsException<PieceHasNoOwnerException>(
        () => _board.GetDropMove(_board.PieceSet[PT.馬], "1a"));

      MyAssert.ThrowsException<ArgumentNullException>(
        () => _board.GetDropMove(null, "1i"));
    }
    [TestMethod]
    public void OneWhoMoves()
    {
      MyAssert.ThrowsException<ArgumentOutOfRangeException>(() =>
      {
        _board.OneWhoMoves = new Board(new StandardPieceSet()).White;
      });
    }
    [TestMethod]
    public void GetPlayerTest()
    {
      Assert.AreSame(_board.White, _board.GetPlayer(PieceColor.White));
      Assert.AreSame(_board.Black, _board.GetPlayer(PieceColor.Black));
    }
    [TestMethod]
    public void CellsPropertyTest()
    {
      Assert.AreEqual(81, _board.Cells.Count());
    }

    // Events: 

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

    #region ' HistoryNavigating/HistoryNavigated Events '

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
      Assert.AreEqual("HistoryNavigating(-2) HistoryNavigated(-2)", log.ToString());
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

    #region ' Implementation '

    private static List<Position> P(params string[] positions)
    {
      return positions.Select(Position.Parse).ToList();
    }

    #endregion
  }
}