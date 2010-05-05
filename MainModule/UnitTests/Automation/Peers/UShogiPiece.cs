using System.Windows.Automation;
using White.Core.UIItems.Actions;
using White.Core.UIItems.Custom;

namespace MainModule.UnitTests.Automation.Peers
{
  [ControlTypeMapping(CustomUIItemType.Custom)]
  public class UShogiPiece : CustomUIItem
  {
    protected UShogiPiece(AutomationElement automationElement, ActionListener actionListener)
      : base(automationElement, actionListener)
    {
    }

    protected UShogiPiece()
    {
    }
  }
}