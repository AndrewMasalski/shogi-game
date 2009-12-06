using System.Collections.Generic;
using System.Linq;
using System.Windows.Automation.Peers;

namespace Yasc.Controls.Automation
{
  public class ShogiHandAutomationPeer : ControlAutomationPeer<ShogiHand>
  {
    public ShogiHandAutomationPeer(ShogiHand hand) 
      : base(hand)
    {
    }

    protected override List<AutomationPeer> GetChildrenCore()
    {
      return new List<AutomationPeer>(
        from i in Owner.Items select CreatePeerForElement(i));
    }

    protected override AutomationControlType GetAutomationControlTypeCore()
    {
      return AutomationControlType.Pane;
    }

    protected override string GetItemTypeCore()
    {
      return typeof(HandNest).Name;
    }
    protected override bool IsContentElementCore()
    {
      return true;
    }

    protected override bool IsControlElementCore()
    {
      return true;
    }
  }
}