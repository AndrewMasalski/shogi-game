using Microsoft.VisualStudio.TestTools.UITesting;
using Yasc.ShogiCore;

namespace BoardControl.AutomationTests.Peers
{
  public class UHandNest : UPieceHolderBase
  {
    public PieceType PieceType { get; set; }

    public UHandNest(UITestControl parent, PieceType pieceType) 
      : base(parent)
    {
      PieceType = pieceType;
      SearchProperties[PropertyNames.Name] = pieceType.ToString();
    }
  }
}