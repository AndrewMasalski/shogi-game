using System.Windows;
using System.Windows.Controls;

namespace Yasc.Common
{
  public class ProgressClock : Control
  {
    static ProgressClock()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressClock), 
        new FrameworkPropertyMetadata(typeof(ProgressClock)));
    }

  }
}
