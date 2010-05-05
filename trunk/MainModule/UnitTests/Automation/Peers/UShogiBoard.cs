using System.Windows.Automation;
using White.Core.UIItems.Actions;
using White.Core.UIItems.Custom;
using White.Core.UIItems.Finders;
using Yasc.ShogiCore;

namespace MainModule.UnitTests.Automation.Peers
{
  [ControlTypeMapping(CustomUIItemType.Custom)]
  public class UShogiBoard : CustomUIItem
  {
    protected UShogiBoard(AutomationElement automationElement, ActionListener actionListener)
      : base(automationElement, actionListener)
    {
    }

    protected UShogiBoard()
    {
    }

    public UShogiCell this[Position p]
    {
      get { return Container.Get<UShogiCell>(SearchCriteria.ByText(p.ToString())); }
    }
    public UShogiHand this[PieceColor player]
    {
      get
      {
        var automationId = player == PieceColor.White ? "TopHand" : "WhiteHand";
        return Container.Get<UShogiHand>(SearchCriteria.ByText(automationId));
      }
    }

    public void UsusalMove(Position from, Position to)
    {
      var cell = this[from];
      var piece = cell.Piece;
      Container.Mouse.DragAndDrop(piece, this[to]);
    }

    public void DropMove(PieceType piece, PieceColor player, Position destination)
    {
      Container.Mouse.DragAndDrop(this[player][piece].Piece, this[destination]);
    }
  }
}