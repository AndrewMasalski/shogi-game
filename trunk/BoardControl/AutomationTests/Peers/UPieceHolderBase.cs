using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;

namespace MainModule.AutomationTests.Peers
{
  public abstract class UPieceHolderBase : WpfCustom
  {
    protected UPieceHolderBase(UITestControl parent) 
      : base(parent)
    {
    }

    public UShogiPiece Piece
    {
      get { return new UShogiPiece(this); }
    }
  }
}