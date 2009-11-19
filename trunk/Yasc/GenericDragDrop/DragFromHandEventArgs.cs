using Yasc.Controls;
using Yasc.ShogiCore;

namespace Yasc.GenericDragDrop
{
  public class DragFromHandEventArgs : DragFromEventArgs
  {
    public ShogiBoard ShogiBoard { get; set; }
    public ShogiPiece ShogiPiece { get; private set; }
    public Piece Piece
    {
      get { return (Piece)ShogiPiece.DataContext; }
    }
    public ShogiHand ShogiHand
    {
      get { return ShogiPiece.FindAncestor<ShogiHand>(); }
    }

    public DragFromHandEventArgs(ShogiBoard shogiBoard, ShogiPiece piece)
    {
      ShogiBoard = shogiBoard;
      ShogiPiece = piece;
    }
  }
}