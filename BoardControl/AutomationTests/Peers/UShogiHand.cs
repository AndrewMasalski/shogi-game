using System;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;
using Yasc.ShogiCore;

namespace MainModule.AutomationTests.Peers
{
  public class UShogiHand : WpfCustom
  {
    public UShogiHand(UITestControl parent, PieceColor player) 
      : base(parent)
    {
      var automationId = player == PieceColor.White ? "TopHand" : "WhiteHand";
      SearchProperties[PropertyNames.Name] = automationId;
    }

    public UHandNest this[PieceType pieceType]
    {
      get
      {
        if (pieceType.IsPromoted) throw new ArgumentOutOfRangeException("pieceType");
        return new UHandNest(this, pieceType);
      }
    }
  }
}