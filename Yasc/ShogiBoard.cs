using System.Windows;
using System.Windows.Controls;

namespace Yasc
{
  public class ShogiBoard : Control
  {
    static ShogiBoard()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ShogiBoard),
        new FrameworkPropertyMetadata(typeof(ShogiBoard)));

    }

    
  }
}
