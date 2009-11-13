using System.Windows;
using System.Windows.Controls;

namespace Yasc.Gui
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