using System.Collections.Generic;
using System.Linq;
using System.Windows.Automation.Peers;

namespace Yasc.Controls.Automation
{
  public class ShogiHandAutomationPeer : FrameworkElementAutomationPeer
  {
    public ShogiHandAutomationPeer(ShogiHand hand) 
      : base(hand)
    {
    }

    public new ShogiHand Owner
    {
      get { return (ShogiHand)base.Owner; }
    }

    protected override List<AutomationPeer> GetChildrenCore()
    {
      return new List<AutomationPeer>(
        from i in Owner.Items select CreatePeerForElement(i));
    }

    protected override string GetClassNameCore()
    {
      return Owner.GetType().Name;
    }

    protected override AutomationControlType GetAutomationControlTypeCore()
    {
      return AutomationControlType.Pane;
    }

    protected override string GetItemTypeCore()
    {
      return typeof(ShogiPiece).Name;
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