using System.Windows;
using System.Windows.Controls;

namespace Yasc.Controls
{
  public class Shield : ContentControl
  {
    static Shield()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(Shield),
        new FrameworkPropertyMetadata(typeof(Shield)));
    }
  }
}
