using Yasc.Controls;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Utils;

namespace Yasc.GenericDragDrop
{
  public class DragFromBoardEventArgs : DragFromEventArgs
  {
    public ShogiPiece ShogiPiece { get; private set; }
    public ShogiCell FromShogiCell { get; private set; }
    public Cell FromCell
    {
      get { return (Cell)FromShogiCell.DataContext; }
    }
    public Piece Piece
    {
      get { return (Piece)ShogiPiece.DataContext; }
    }
    public Position FromPosition
    {
      get { return FromCell.Position; }
    }

    public DragFromBoardEventArgs(ShogiPiece piece, ShogiCell cell)
    {
      ShogiPiece = piece;
      FromShogiCell = cell;
    }
  }
}