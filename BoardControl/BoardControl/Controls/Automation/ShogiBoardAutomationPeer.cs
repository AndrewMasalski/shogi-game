using System.Collections.Generic;
using System.Windows.Automation.Peers;

namespace Yasc.BoardControl.Controls.Automation
{
  public class ShogiBoardAutomationPeer : ControlAutomationPeer<ShogiBoard>
  {
    public ShogiBoardAutomationPeer(ShogiBoard owner)
      : base(owner)
    {
    }

    protected override List<AutomationPeer> GetChildrenCore()
    {
      var automationPeers = new List<AutomationPeer>();
      if (Owner.AreChildrenLoaded)
      {
        automationPeers.Add(CreatePeerForElement(Owner.WhiteHand));
        automationPeers.Add(CreatePeerForElement(Owner.BlackHand));
        automationPeers.Add(CreatePeerForElement(Owner.Core));
      }
      return automationPeers;
    }
  }
}