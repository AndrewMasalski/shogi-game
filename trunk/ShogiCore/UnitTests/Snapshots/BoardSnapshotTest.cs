using System;
using System.Collections.Generic;
using CommonUtils.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.Notations;
using Yasc.ShogiCore.PieceSets;
using Yasc.ShogiCore.Primitives;
using Yasc.ShogiCore.Snapshots;
using System.Linq;

namespace ShogiCore.UnitTests.Snapshots
{
  [TestClass]
  public class BoardSnapshotTest
  {
    private BoardSnapshot _snapshot;

    [TestInitialize]
    public void Init()
    {
      var board = new Board(new StandardPieceSet());
      board.LoadSnapshotWithoutHistory(BoardSnapshot.InitialPosition);
      _snapshot = board.CurrentSnapshot;
    }

    [TestMethod]
    public void Immutability()
    {
      var clone = _snapshot.MakeMove(new UsualMove(_snapshot,
        PieceColor.White, Position.Parse("3g"), Position.Parse("3f"), false));
      
      Assert.IsNull(clone.GetPieceAt("3g"));
      Assert.IsNotNull(clone.GetPieceAt("3f"));

      Assert.IsNotNull(_snapshot.GetPieceAt("3g"));
      Assert.IsNull(_snapshot.GetPieceAt("3f"));
    }
    [TestMethod]
    public void CollectionsReadonly()
    {
      MyAssert.ThrowsException<NotSupportedException>(
        () => ((IList<IColoredPiece>)_snapshot.Cells).Add(PT.馬.Black));

      MyAssert.ThrowsException<NotSupportedException>(
        () => ((IList<IPieceType>)_snapshot.BlackHand).Add(PT.馬));

      MyAssert.ThrowsException<NotSupportedException>(
        () => ((IList<IPieceType>)_snapshot.WhiteHand).Add(PT.馬));
    }
    [TestMethod]
    public void HandIndexerTest()
    {
      Assert.AreSame(_snapshot.WhiteHand, _snapshot.GetHand(PieceColor.White));
      Assert.AreSame(_snapshot.BlackHand, _snapshot.GetHand(PieceColor.Black));
    }
    [TestMethod]
    public void GetPieceAtOverloadsTest()
    {
      foreach (var p in Position.OnBoard)
        Assert.AreSame(
          _snapshot.Cells[p.X, p.Y], 
          _snapshot.GetPieceAt(p));
    }
    [TestMethod]
    public void EqualityTest()
    {
      var snapshot1 = _snapshot;
      var snapshot2 = _snapshot.SerializeDeserialize();

      Assert.AreNotSame(snapshot1, snapshot2);
      Assert.IsTrue(snapshot1.Equals(snapshot2));
      Assert.IsTrue(snapshot1.Equals((object)snapshot2));
      Assert.IsFalse(snapshot1.Equals(new object()));
      Assert.IsFalse(snapshot1.Equals(null));
      Assert.IsTrue(snapshot1 == snapshot2);
      Assert.IsFalse(snapshot1 != snapshot2);
    }
    [TestMethod]
    public void IsCheckForTest()
    {
      Assert.IsFalse(_snapshot.IsCheckFor(PieceColor.Black));
    }
    [TestMethod]
    public void ParseSfenExceptions()
    {
      MyAssert.ThrowsException<ArgumentNullException>(
        () => BoardSnapshot.ParseSfen(null));

      MyAssert.ThrowsException<ArgumentOutOfRangeException>(
        () => BoardSnapshot.ParseSfen(""));

      MyAssert.ThrowsException<ArgumentOutOfRangeException>(
        () => BoardSnapshot.ParseSfen("a b c"));
    }

    [TestMethod]
    public void ParseSfen()
    {
      Assert.AreEqual(BoardSnapshot.InitialPosition, 
        BoardSnapshot.ParseSfen("lnsgkgsnl/1r5b1/ppppppppp/9/9/9/PPPPPPPPP/1B5R1/LNSGKGSNL b - 1"));

      Assert.AreEqual(new BoardSnapshot(PieceColor.White, 
        new[]
          {
            Tuple.Create(Position.Parse("1a"), PT.香.White),
            Tuple.Create(Position.Parse("7a"), PT.銀.White),
            Tuple.Create(Position.Parse("8a"), PT.桂.White),
            Tuple.Create(Position.Parse("9a"), PT.香.White),
            Tuple.Create(Position.Parse("8b"), PT.金.Black),
            Tuple.Create(Position.Parse("5c"), PT.歩.White),
            Tuple.Create(Position.Parse("6c"), PT.歩.White),
            Tuple.Create(Position.Parse("7c"), PT.歩.White),
            Tuple.Create(Position.Parse("8c"), PT.歩.White),
            Tuple.Create(Position.Parse("1d"), PT.歩.White),
            Tuple.Create(Position.Parse("4d"), PT.飛.Black),
            Tuple.Create(Position.Parse("9d"), PT.歩.White),
            Tuple.Create(Position.Parse("2e"), PT.歩.White),
            Tuple.Create(Position.Parse("6e"), PT.歩.Black),
            Tuple.Create(Position.Parse("7e"), PT.角.Black),
            Tuple.Create(Position.Parse("1f"), PT.歩.Black),
            Tuple.Create(Position.Parse("2f"), PT.歩.Black),
            Tuple.Create(Position.Parse("8f"), PT.銀.Black),
            Tuple.Create(Position.Parse("9f"), PT.歩.Black),
            Tuple.Create(Position.Parse("4g"), PT.と.White),
            Tuple.Create(Position.Parse("5g"), PT.歩.Black),
            Tuple.Create(Position.Parse("7g"), PT.銀.Black),
            Tuple.Create(Position.Parse("8g"), PT.王.White),
            Tuple.Create(Position.Parse("6h"), PT.金.Black),
            Tuple.Create(Position.Parse("8h"), PT.金.Black),
            Tuple.Create(Position.Parse("1i"), PT.馬.White),
            Tuple.Create(Position.Parse("2i"), PT.桂.Black),
            Tuple.Create(Position.Parse("6i"), PT.玉.Black),
            Tuple.Create(Position.Parse("8i"), PT.桂.Black),
            Tuple.Create(Position.Parse("9i"), PT.香.Black),
          },
          new[] { PT.歩,PT.歩,PT.歩,PT.香,PT.桂,PT.飛},
          new[] { PT.歩,PT.歩,PT.銀,PT.金}),
        BoardSnapshot.ParseSfen("lns5l/1G7/1pppp4/p4R2p/2BP3p1/PS5PP/1kS1P+p3/1G1G5/LN1K3N+b w GS2Prnl3p 1"));
    }
    [TestMethod]
    public void ToSfenString()
    {
      Assert.AreEqual("lnsgkgsnl/1r5b1/ppppppppp/9/9/9/PPPPPPPPP/1B5R1/LNSGKGSNL B - 1",
        BoardSnapshot.InitialPosition.ToSfenString());

      const string sfenExample = "lns5l/1G7/1pppp4/p4R2p/2BP3p1/PS5PP/1kS1P+p3/1G1G5/LN1K3N+b W GS2Prnl3p 1";
      Assert.AreEqual(sfenExample, BoardSnapshot.ParseSfen(sfenExample).ToSfenString());
    }
    [TestMethod]
    public void PieceDemotesAfterTake()
    {
      var board = BoardSnapshot.ParseSfen("7+r+R/9/9/9/9/9/9/9/9 B -");
      var move = CuteNotation.Instance.Parse(board, "+Rx").First();
      var after = board.MakeMove(move);
      Assert.AreEqual(PT.飛, after.BlackHand.First());
    }
  }
}