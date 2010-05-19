using Microsoft.VisualStudio.TestTools.UITesting;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Primitives;

namespace BoardControl.AutomationTests.Peers
{ 
  public class UShogiCell : UPieceHolderBase
  {
    public Position Position { get; set; }

    public UShogiCell(UITestControl parent, Position position) 
      : base(parent)
    {
      Position = position;
      SearchProperties[PropertyNames.Name] = position.ToString();
    }
  }
}