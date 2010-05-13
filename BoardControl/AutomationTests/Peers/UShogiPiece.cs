using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;
using Yasc.BoardControl.Controls;

namespace MainModule.AutomationTests.Peers
{
  public class UShogiPiece : WpfCustom
  {
    public UShogiPiece(UITestControl parent) 
      : base(parent)
    {
      SearchProperties[PropertyNames.ClassName] = typeof (ShogiPiece).UiaClassName();
    }
  }
}