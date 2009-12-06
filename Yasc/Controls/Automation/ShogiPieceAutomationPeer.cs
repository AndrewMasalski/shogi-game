using System.Windows.Automation.Peers;

namespace Yasc.Controls.Automation
{
  public class ShogiPieceAutomationPeer : FrameworkElementAutomationPeer
  {
    public ShogiPieceAutomationPeer(ShogiPiece owner)
      : base(owner)
    {
    }


    public new ShogiPiece Owner
    {
      get { return (ShogiPiece)base.Owner; }
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
      return false;
    }

    protected override bool IsControlElementCore()
    {
      return true;
    }
  }
}