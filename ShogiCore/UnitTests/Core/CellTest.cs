using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.PieceSets;
using Yasc.ShogiCore.Primitives;

namespace ShogiCore.UnitTests.Core
{
  [TestClass]
  public class CellTest
  {
    private Board _board;

    [TestInitialize]
    public void Init()
    {
      _board = new Board(new StandardPieceSet());
    }

    [TestMethod]
    public void Owner()
    {
      foreach (var cell in _board.Cells)
        Assert.AreSame(_board, cell.Owner);
    }
    [TestMethod]
    public void PositionTest()
    {
      foreach (var position in Position.OnBoard)
        Assert.AreEqual(position, _board.GetCellAt(position).Position);
    }
  }
}