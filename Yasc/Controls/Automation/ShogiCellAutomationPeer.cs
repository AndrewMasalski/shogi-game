using System.Collections.Generic;
using System.Windows.Automation.Peers;

namespace Yasc.Controls.Automation
{
  public class ShogiCellAutomationPeer : FrameworkElementAutomationPeer
  {
    public ShogiCellAutomationPeer(ShogiCell owner)
      : base(owner)
    {
    }


    public new ShogiCell Owner
    {
      get { return (ShogiCell)base.Owner; }
    }

    protected override List<AutomationPeer> GetChildrenCore()
    {
      var peers = new List<AutomationPeer>();
      var piece = Owner.ShogiPiece;
      if (piece != null)
        peers.Add(CreatePeerForElement(piece));
      return peers;
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