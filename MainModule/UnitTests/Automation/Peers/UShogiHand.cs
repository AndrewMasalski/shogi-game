using System;
using System.Windows.Automation;
using White.Core.UIItems.Actions;
using White.Core.UIItems.Custom;
using White.Core.UIItems.Finders;
using Yasc.ShogiCore;

namespace MainModule.UnitTests.Automation.Peers
{
  [ControlTypeMapping(CustomUIItemType.Custom)]
  public class UShogiHand : CustomUIItem
  {
    protected UShogiHand(AutomationElement automationElement, ActionListener actionListener)
      : base(automationElement, actionListener)
    {
    }

    protected UShogiHand()
    {
    }

    public UHandNest this[PieceType pieceType]
    {
      get
      {
        if (pieceType.IsPromoted) throw new ArgumentOutOfRangeException("pieceType");
        return Container.Get<UHandNest>(SearchCriteria.ByText(pieceType.ToString()));
      }
    }
  }
}