using System.Windows.Automation;
using Yasc.Controls;
using Yasc.ShogiCore;

namespace UnitTests.Automation
{
  public class ShogiHandAutomation : ConcreteAutomation<ShogiHand>
  {
    public ShogiHandAutomation(AutomationElement element)
      : base(element)
    {
    }

    public HandNestAutomation this[PieceType pieceType]
    {
      get { return new HandNestAutomation(Element.FindFirstByName(pieceType)); }
    }
  }
}