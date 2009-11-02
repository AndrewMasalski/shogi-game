using System.Windows;
using System.Windows.Controls;

namespace Yasc
{
  public class ShogiBoardCore : Control
  {
    static ShogiBoardCore()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ShogiBoardCore), 
        new FrameworkPropertyMetadata(typeof(ShogiBoardCore)));
    }
  }
}
