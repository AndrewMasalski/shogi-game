using System.Collections.Generic;
using System.Windows.Automation.Peers;

namespace Yasc.Controls.Automation
{
  public class ShogiBoardAutomationPeer : FrameworkElementAutomationPeer
  {
    public ShogiBoardAutomationPeer(ShogiBoard owner)
      : base(owner)
    {
    }
    public new ShogiBoard Owner
    {
      get { return (ShogiBoard)base.Owner; }
    }
    protected override string GetClassNameCore()
    {
      return Owner.GetType().Name;
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