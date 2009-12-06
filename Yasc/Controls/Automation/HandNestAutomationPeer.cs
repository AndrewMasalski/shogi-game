using System.Collections.Generic;
using System.Windows.Automation.Peers;

namespace Yasc.Controls.Automation
{
  public class HandNestAutomationPeer : ControlAutomationPeer<HandNest>
  {
    public HandNestAutomationPeer(HandNest owner) 
      : base(owner)
    {
    }

    protected override List<AutomationPeer> GetChildrenCore()
    {
      return new List<AutomationPeer>
               {
                 CreatePeerForElement(Owner.ShogiPiece)
               };
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