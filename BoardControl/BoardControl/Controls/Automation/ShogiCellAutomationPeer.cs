using System.Collections.Generic;
using System.Windows.Automation.Peers;

namespace Yasc.BoardControl.Controls.Automation
{
  public class ShogiCellAutomationPeer : ControlAutomationPeer<ShogiCell>
  {
    public ShogiCellAutomationPeer(ShogiCell owner)
      : base(owner)
    {
    }

    protected override List<AutomationPeer> GetChildrenCore()
    {
      var peers = new List<AutomationPeer>();
      var piece = Owner.ShogiPiece;
      if (piece != null)
        peers.Add(CreatePeerForElement(piece));
      return peers;
    }

    protected override string GetItemTypeCore()
    {
      return typeof(ShogiPiece).Name;
    }

    protected override string GetNameCore()
    {
      return Owner.Cell.Position.ToString();
    }
  }
}