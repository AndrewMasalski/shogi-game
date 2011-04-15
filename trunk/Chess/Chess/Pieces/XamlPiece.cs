using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Chess.Pieces
{
  public class XamlPiece : UserControl
  {
    private Size _originalSize;

    protected override void OnInitialized(EventArgs e)
    {
      base.OnInitialized(e);
      _originalSize = new Size(Width, Height);
      ClearValue(WidthProperty);
      ClearValue(HeightProperty);

    }
    protected override Size MeasureOverride(Size constraint)
    {
      var measureOverride = base.MeasureOverride(constraint);
      return measureOverride;
    }

    protected override Size ArrangeOverride(Size arrangeBounds)
    {
      var fe = (FrameworkElement)Content;
      var scale = Math.Min(
        arrangeBounds.Width / _originalSize.Width,
        arrangeBounds.Height / _originalSize.Height);
      scale *= .66 ;
      var newSize = (System.Windows.Vector)_originalSize * scale;
//      var spare = ((System.Windows.Vector)arrangeBounds - newSize) / 2;
//      spare -= spare;
//      fe.RenderTransformOrigin = new Point(.5, .5);
      fe.RenderTransform = new ScaleTransform(scale, scale);
      fe.Arrange(new Rect(new Point(), newSize));
      return (Size)newSize;
    }
  }
}
