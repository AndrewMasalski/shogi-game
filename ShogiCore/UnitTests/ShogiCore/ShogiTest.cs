using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.Primitives;

namespace ShogiCore.UnitTests.ShogiCore
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
        Assert.AreEqual(pair.Value, (string)_board[pair.Key].PieceType);
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
            Assert.AreEqual(Shogi.InitialPosition[cell], (string)placeHolder.Piece.PieceType);
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
}