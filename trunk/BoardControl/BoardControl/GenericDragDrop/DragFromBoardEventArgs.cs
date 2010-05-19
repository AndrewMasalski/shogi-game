using Yasc.BoardControl.Controls;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.Primitives;

namespace Yasc.BoardControl.GenericDragDrop
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
      get { return ShogiPiece.Piece; }
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