using System.Windows.Automation;
using Yasc.Controls;

namespace UnitTests.Automation
{
  public class HandNestAutomation : ConcreteAutomation<HandNest>
  {
    public HandNestAutomation(AutomationElement element) 
      : base(element)
    {
    }
  }
}