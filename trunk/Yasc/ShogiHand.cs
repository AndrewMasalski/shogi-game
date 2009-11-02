using System.Windows;
using System.Windows.Controls;

namespace Yasc
{
  public class ShogiHand : Control
  {
    static ShogiHand()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ShogiHand),
        new FrameworkPropertyMetadata(typeof(ShogiHand)));
    }
  }
}
