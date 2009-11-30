using Yasc.Controls;
using Yasc.ShogiCore;

namespace Yasc.GenericDragDrop
{
  public class DragFromHandEventArgs : DragFromEventArgs
  {
    public ShogiBoard ShogiBoard { get; set; }
    public ShogiPiece ShogiPiece { get; private set; }
    public PieceType PieceType
    {
      get { return ShogiPiece.PieceType; }
    }
    public ShogiHand ShogiHand
    {
      get { return ShogiPiece.FindAncestor<ShogiHand>(); }
    }

    public PieceColor PieceColor
    {
      get { return ShogiPiece.PieceColor; }
    }

    public DragFromHandEventArgs(ShogiBoard shogiBoard, ShogiPiece piece)
    {
      ShogiBoard = shogiBoard;
      ShogiPiece = piece;
    }
  }
}