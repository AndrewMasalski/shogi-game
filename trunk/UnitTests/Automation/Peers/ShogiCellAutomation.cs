using System.Windows.Automation;
using Yasc.Controls;

namespace UnitTests.Automation
{
  public class ShogiCellAutomation : ConcreteAutomation<ShogiCell>
  {
    public ShogiCellAutomation(AutomationElement element)
      : base(element)
    {
    }
  }
}