
namespace Yasc.BoardControl.Controls.Automation
{
  public class ShogiPieceAutomationPeer : ControlAutomationPeer<ShogiPiece>
  {
    public ShogiPieceAutomationPeer(ShogiPiece owner)
      : base(owner)
    {
    }

    protected override string GetNameCore()
    {
      return Owner.ToLatinString();
    }
    protected override bool IsContentElementCore()
    {
      return false;
    }
  }
}