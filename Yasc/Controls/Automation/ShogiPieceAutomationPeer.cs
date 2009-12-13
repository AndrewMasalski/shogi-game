using System.Diagnostics;

namespace Yasc.Controls.Automation
{
  public class ShogiPieceAutomationPeer : ControlAutomationPeer<ShogiPiece>
  {
    public ShogiPieceAutomationPeer(ShogiPiece owner)
      : base(owner)
    {
      Debug.Assert(owner.Piece != null);
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