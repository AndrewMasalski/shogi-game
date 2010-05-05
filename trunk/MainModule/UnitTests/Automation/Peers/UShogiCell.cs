using System.Windows.Automation;
using White.Core.UIItems.Actions;
using White.Core.UIItems.Custom;

namespace MainModule.UnitTests.Automation.Peers
{ 
  [ControlTypeMapping(CustomUIItemType.Custom)]
  public class UShogiCell : UPieceHolderBase
  {
    protected UShogiCell(AutomationElement automationElement, ActionListener actionListener)
      : base(automationElement, actionListener)
    {
    }

    protected UShogiCell()
    {
    }
  }
}