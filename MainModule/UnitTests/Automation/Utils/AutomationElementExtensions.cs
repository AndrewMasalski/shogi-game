using System.Windows;
using System.Windows.Automation;

namespace UnitTests.Automation.Utils
{
  public static class AutomationElementExtensions
  {
    public static Point Center(this AutomationElement element)
    {
      return element.Current.BoundingRectangle.Center();
    }
  }
}