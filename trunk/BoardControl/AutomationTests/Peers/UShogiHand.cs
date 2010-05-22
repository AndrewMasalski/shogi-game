using System;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Primitives;

namespace BoardControl.AutomationTests.Peers
{
  public class UShogiHand : WpfCustom
  {
    public UShogiHand(UITestControl parent, PieceColor player) 
      : base(parent)
    {
      var automationId = player == PieceColor.White ? "TopHand" : "WhiteHand";
      SearchProperties[PropertyNames.Name] = automationId;
    }

    public UHandNest this[IPieceType pieceType]
    {
      get
      {
        if (pieceType.IsPromoted) throw new ArgumentOutOfRangeException("pieceType");
        return new UHandNest(this, pieceType);
      }
    }
  }
}