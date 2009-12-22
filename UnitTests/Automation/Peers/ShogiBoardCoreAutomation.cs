using System.Windows.Automation;
using Yasc.Controls;

namespace UnitTests.Automation
{
  public class ShogiBoardCoreAutomation : ConcreteAutomation<ShogiBoardCore>
  {
    public ShogiBoardCoreAutomation(AutomationElement element) 
      : base(element)
    {
    }
  }
}