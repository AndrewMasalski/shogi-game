using System.Windows;
using System.Windows.Controls;

namespace Yasc.Controls
{
  public class BoardBorder : ContentControl
  {
    static BoardBorder()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(BoardBorder), 
        new FrameworkPropertyMetadata(typeof(BoardBorder)));
    }

    public static readonly DependencyProperty IsFlippedProperty = ShogiBoard.IsFlippedProperty.AddOwner(
      typeof (BoardBorder), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

    public bool IsFlipped
    {
      get { return (bool)GetValue(IsFlippedProperty); }
      set { SetValue(IsFlippedProperty, value); }
    }
  }
}
