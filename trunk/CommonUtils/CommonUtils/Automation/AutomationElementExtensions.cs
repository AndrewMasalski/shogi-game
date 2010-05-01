using System.Windows;
using System.Windows.Automation;

namespace Yasc.Utils.Automation
{
  public static class AutomationElementExtensions
  {
    public static Point Center(this AutomationElement element)
    {
      return element.Current.BoundingRectangle.Center();
    }
  }
}