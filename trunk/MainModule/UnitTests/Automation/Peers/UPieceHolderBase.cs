using System.Windows.Automation;
using White.Core.UIItems.Actions;
using White.Core.UIItems.Custom;
using White.Core.UIItems.Finders;

namespace MainModule.UnitTests.Automation.Peers
{
  [ControlTypeMapping(CustomUIItemType.Custom)]
  public abstract class UPieceHolderBase : CustomUIItem
  {
    protected UPieceHolderBase(AutomationElement automationElement, ActionListener actionListener)
      : base(automationElement, actionListener)
    {
    }

    protected UPieceHolderBase()
    {
    }

    public UShogiPiece Piece
    {
      get { return Container.Get<UShogiPiece>(SearchCriteria.All); }
    }
  }
}