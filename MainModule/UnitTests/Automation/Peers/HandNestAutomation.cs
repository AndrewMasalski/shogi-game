using System.Windows.Automation;
using Yasc.BoardControl.Controls;

namespace MainModule.UnitTests.Automation.Peers
{
  public class HandNestAutomation : ConcreteAutomation<HandNest>
  {
    public HandNestAutomation(AutomationElement element) 
      : base(element)
    {
    }
  }
}