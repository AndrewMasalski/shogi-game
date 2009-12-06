using System.Collections.Generic;
using System.Windows.Automation.Peers;

namespace Yasc.Controls.Automation
{
  public class HandNestAutomationPeer : FrameworkElementAutomationPeer
  {
    public HandNestAutomationPeer(HandNest owner) 
      : base(owner)
    {
    }


    public new HandNest Owner
    {
      get { return (HandNest)base.Owner; }
    }

    protected override List<AutomationPeer> GetChildrenCore()
    {
      return new List<AutomationPeer>
               {
                 CreatePeerForElement(Owner.ShogiPiece)
               };
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