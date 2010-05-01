using System.Windows.Automation;
using Yasc.BoardControl.Controls;

namespace MainModule.UnitTests.Automation.Peers
{
  public class ShogiBoardCoreAutomation : ConcreteAutomation<ShogiBoardCore>
  {
    public ShogiBoardCoreAutomation(AutomationElement element) 
      : base(element)
    {
    }
  }
}