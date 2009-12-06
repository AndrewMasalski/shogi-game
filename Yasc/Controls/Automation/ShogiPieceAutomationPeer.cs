using System.Windows.Automation.Peers;

namespace Yasc.Controls.Automation
{
  public class ShogiPieceAutomationPeer : ControlAutomationPeer<ShogiPiece>
  {
    public ShogiPieceAutomationPeer(ShogiPiece owner)
      : base(owner)
    {
    }

    protected override AutomationControlType GetAutomationControlTypeCore()
    {
      return AutomationControlType.Pane;
    }
    protected override bool IsContentElementCore()
    {
      return false;
    }
    protected override bool IsControlElementCore()
    {
      return true;
    }
  }
}