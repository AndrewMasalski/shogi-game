using Yasc.BoardControl.Controls;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Primitives;

namespace Yasc.BoardControl.GenericDragDrop
{
  public class DragFromHandEventArgs : DragFromEventArgs
  {
    public ShogiBoard ShogiBoard { get; private set; }
    public ShogiPiece ShogiPiece { get; private set; }
    public IPieceType PieceType
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