using System.Windows.Automation;
using Yasc.Controls;

namespace UnitTests.Automation.Peers
{
  public class HandNestAutomation : ConcreteAutomation<HandNest>
  {
    public HandNestAutomation(AutomationElement element) 
      : base(element)
    {
    }
  }
}