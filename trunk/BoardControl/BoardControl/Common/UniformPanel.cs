using System;
using System.Windows;
using System.Windows.Controls;

namespace Yasc.BoardControl.Common
{
  public class UniformPanel : Panel
  {
    protected override Size MeasureOverride(Size availableSize)
    {
      double size = Math.Min(availableSize.Width, availableSize.Height);
      if (double.IsNaN(size) || double.IsInfinity(size))
        size = 1000;
      var result = new Size(size, size);
      foreach (FrameworkElement child in InternalChildren)
        child.Measure(result);
      return result;
    }
    protected override Size ArrangeOverride(Size finalSize)
    {
      foreach (FrameworkElement child in InternalChildren)
      {
        var d = Diff(finalSize, child.DesiredSize) / 2;
        child.Arrange(new Rect((Point)d, child.DesiredSize));
      }
      return finalSize;
    }
    private static Vector Diff(Size a, Size b)
    {
      return new Vector(a.Width - b.Width, a.Height - b.Height);
    }
  }
}