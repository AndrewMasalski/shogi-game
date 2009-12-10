using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Yasc.Common
{
  public class UniformPanel : Panel
  {
    protected override Size MeasureOverride(Size constraint)
    {
      double size = Math.Min(constraint.Width, constraint.Height);
      if (double.IsNaN(size) || double.IsInfinity(size))
        size = 1000;
      var availableSize = new Size(size, size);
      foreach (FrameworkElement child in InternalChildren)
        child.Measure(availableSize);
      return availableSize;
    }
    protected override Size ArrangeOverride(Size arrangeBounds)
    {
      foreach (FrameworkElement child in InternalChildren)
      {
        var d = Diff(arrangeBounds, child.DesiredSize) / 2;
        child.Arrange(new Rect((Point)d, child.DesiredSize));
      }
      return arrangeBounds;
    }
    private static Vector Diff(Size a, Size b)
    {
      return new Vector(a.Width - b.Width, a.Height - b.Height);
    }
  }
}