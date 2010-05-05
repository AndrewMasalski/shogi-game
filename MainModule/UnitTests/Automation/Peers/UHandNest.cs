using System.Windows.Automation;
using White.Core.UIItems.Actions;
using White.Core.UIItems.Custom;

namespace MainModule.UnitTests.Automation.Peers
{
  [ControlTypeMapping(CustomUIItemType.Custom)]
  public class UHandNest : UPieceHolderBase
  {
    protected UHandNest(AutomationElement automationElement, ActionListener actionListener)
      : base(automationElement, actionListener)
    {
    }

    protected UHandNest()
    {
    }
  }
}