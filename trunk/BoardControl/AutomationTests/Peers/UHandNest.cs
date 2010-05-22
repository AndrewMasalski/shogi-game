using Microsoft.VisualStudio.TestTools.UITesting;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Primitives;

namespace BoardControl.AutomationTests.Peers
{
  public class UHandNest : UPieceHolderBase
  {
    public IPieceType PieceType { get; set; }

    public UHandNest(UITestControl parent, IPieceType pieceType) 
      : base(parent)
    {
      PieceType = pieceType;
      SearchProperties[PropertyNames.Name] = pieceType.ToString();
    }
  }
}