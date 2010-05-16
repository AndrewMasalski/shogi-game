using System;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;
using System.Linq;

namespace BoardControl.AutomationTests.Peers
{
  public static class UITestControlExtensions
  {
    public static void InvokeMenu(this UITestControl ctrl, string menuPath)
    {
      var menu = new WpfMenu(ctrl);
      var path = menuPath.Split('/');

      var submenu = new Func<WpfControl, string, WpfMenuItem>(
        (control, submenuName) =>
          {
            var result = new WpfMenuItem(control);
            result.SearchProperties[WpfMenu.PropertyNames.Name] = submenuName;
            result.EnsureClickable();
            Mouse.Click(result);
            return result;
          });

      var item = submenu(menu, path[0]);
      foreach (var pathElement in path.Skip(1))
        submenu(item, pathElement);
    }
  }
}