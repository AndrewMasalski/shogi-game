using System.Windows.Automation;
using Yasc.BoardControl.Controls;

namespace MainModule.UnitTests.Automation.Peers
{
  public class ShogiCellAutomation : ConcreteAutomation<ShogiCell>
  {
    public ShogiCellAutomation(AutomationElement element)
      : base(element)
    {
    }
  }
}