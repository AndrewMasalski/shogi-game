using System.Collections.Generic;
using System.Windows.Automation.Peers;

namespace Yasc.Controls.Automation
{
  public class ShogiBoardAutomationPeer : ControlAutomationPeer<ShogiBoard>
  {
    public ShogiBoardAutomationPeer(ShogiBoard owner)
      : base(owner)
    {
    }

    protected override List<AutomationPeer> GetChildrenCore()
    {
      return new List<AutomationPeer>
               {
                 CreatePeerForElement(Owner.WhiteHand),
                 CreatePeerForElement(Owner.BlackHand),
                 CreatePeerForElement(Owner.Core)
               };
    }

    protected override AutomationControlType GetAutomationControlTypeCore()
    {
      return AutomationControlType.Pane;
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