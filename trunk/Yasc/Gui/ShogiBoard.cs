using System.Windows;
using System.Windows.Controls;

namespace Yasc.Gui
{
  public class ShogiBoard : Control
  {
    static ShogiBoard()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ShogiBoard),
        new FrameworkPropertyMetadata(typeof(ShogiBoard)));
    }

//    public override void OnApplyTemplate()
//    {
//      base.OnApplyTemplate();
//      var boardCore = GetTemplateChild("PART_BoardCore") as ContentControl;
//      if (boardCore != null)
//      {
//        boardCore.Content = new Canvas();
//      }
//    }
  }
}