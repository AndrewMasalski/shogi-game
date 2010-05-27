using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.PieceSets;
using Yasc.ShogiCore.Primitives;

namespace ShogiCore.UnitTests.Core
{
  [TestClass]
  public class CellTest
  {
    // TODO: Cell.Piece, Cell.NotifyPropertyChanged("Piece")

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
    [TestMethod]
    public void Piece()
    {
      var cell = _board.GetCellAt(0, 0);
      Assert.IsNull(cell.Piece);
      _board.ResetPiece(cell.Position);
      Assert.IsNull(cell.Piece);
      var piece = _board.PieceSet[PT.馬];
      _board.SetPiece(piece, cell.Position, _board.White);
      Assert.AreSame(cell.Piece, piece);
      _board.SetPiece(piece, cell.Position);
      Assert.AreSame(cell.Piece, piece);
    }
  }
}