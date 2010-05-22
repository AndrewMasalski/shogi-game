using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.Primitives;
using Yasc.ShogiCore.Snapshots;
using Yasc.Utils;

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

      /// <summary>Contains position-pieceType pairs of initial position</summary>
      private static readonly ReadOnlyDictionary<Position, PieceType> _initialPosition = new[]
          {
            "1a", "香", "9a", "香", "1i", "香", "9i", "香",
            "2a", "桂", "8a", "桂", "2i", "桂", "8i", "桂",
            "3a", "銀", "7a", "銀", "3i", "銀", "7i", "銀",
            "4a", "金", "6a", "金", "4i", "金", "6i", "金",
            "5a", "王", "5i", "玉", "2b", "角", "8h", "角",
            "8b", "飛", "2h", "飛", "1c", "歩", "2c", "歩",
            "3c", "歩", "4c", "歩", "5c", "歩", "6c", "歩",
            "7c", "歩", "8c", "歩", "9c", "歩", "1g", "歩",
            "2g", "歩", "3g", "歩", "4g", "歩", "5g", "歩",
            "6g", "歩", "7g", "歩", "8g", "歩", "9g", "歩",
          }
            .Pairs((position, pieceType) => Tuple.Create(
              Position.Parse(position), PieceType.Parse(pieceType)))
            .ToReadOnlyDictionary(pair => pair.Item1, pair => pair.Item2);


    [TestMethod]
    public void InitialPositionTest()
    {
      //    ______________
      //___/ Create board \__________________________________________________
      _board.LoadSnapshot(BoardSnapshot.InitialPosition);
      //    _________________________________
      //___/ Check pieces which must present \_______________________________
      foreach (var pair in _initialPosition)
        Assert.AreEqual(pair.Value, (string)_board.GetPieceAt(pair.Key).PieceType);
      //    _________________________________
      //___/ Check cells which must be empty \_______________________________
      for (int i = 0; i < 9; i++)
        for (int j = 0; j < 9; j++)
          if (!_initialPosition.ContainsKey(new Position(i, j)))
            Assert.IsNull(_board.GetCellAt(i, j).Piece);
    }
    [TestMethod]
    public void CellBindabilityTest()
    {
      int counter = 0;
      var handler = new Action<Position, Cell>(
        (cell, placeHolder) =>
          {
            counter++;
            Assert.AreEqual(_initialPosition[cell], (string)placeHolder.Piece.PieceType);
          });

      for (int i = 0; i < 9; i++)
        for (int j = 0; j < 9; j++)
        {
          int x = i;
          int y = j;
          _board.GetCellAt(i, j).PropertyChanged +=
            (s, e) => handler(new Position(x, y), (Cell)s);
        }

      _board.LoadSnapshot(BoardSnapshot.InitialPosition);
      Assert.AreEqual(counter, _initialPosition.Count());
    }
  }
}