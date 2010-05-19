using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;
using Yasc.BoardControl.Controls;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Primitives;

namespace BoardControl.AutomationTests.Peers
{
  public class UShogiBoard : WpfCustom
  {
    public UShogiBoard(UITestControl parent) 
      : base(parent)
    {
      SearchProperties[PropertyNames.ClassName] = typeof(ShogiBoard).UiaClassName();
    }

    public UShogiCell this[Position p]
    {
      get { return new UShogiCell(this, p); }
    }
    public UShogiHand this[PieceColor player]
    {
      get { return new UShogiHand(this, player); }
    }

    public void UsusalMove(Position from, Position to)
    {
      var cell = this[from];
      var piece = cell.Piece;
      Mouse.StartDragging(piece);
      Mouse.StopDragging(this[to]);
    }

    public void DropMove(PieceType pieceType, PieceColor player, Position destination)
    {
      Mouse.StartDragging(this[player][pieceType].Piece);
      Mouse.StopDragging(this[destination]);
    }
  }
}