using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Chess
{
  public class BoardPanel : Panel
  {
    #region ' Position : Position Attached Property '

    public static readonly DependencyProperty PositionProperty = DependencyProperty.
      RegisterAttached("Position", typeof(Position), typeof(BoardPanel),
                       new FrameworkPropertyMetadata(default(Position), OnPositionChanged));

    public static Position GetPosition(DependencyObject obj)
    {
      return (Position)obj.GetValue(PositionProperty);
    }

    public static void SetPosition(DependencyObject obj, Position value)
    {
      obj.SetValue(PositionProperty, value);
    }

    private static void OnPositionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {

    }

    #endregion

    #region ' Column : int Attached Property '

    public static readonly DependencyProperty ColumnProperty = DependencyProperty.
      RegisterAttached("Column", typeof(int), typeof(BoardPanel),
                       new FrameworkPropertyMetadata(-1, OnColumnChanged));

    public static int GetColumn(DependencyObject obj)
    {
      return (int)obj.GetValue(ColumnProperty);
    }

    public static void SetColumn(DependencyObject obj, int value)
    {
      obj.SetValue(ColumnProperty, value);
    }

    private static void OnColumnChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {

    }

    #endregion

    #region ' Row : int Attached Property '

    public static readonly DependencyProperty RowProperty = DependencyProperty.
      RegisterAttached("Row", typeof(int), typeof(BoardPanel),
                       new FrameworkPropertyMetadata(-1, OnRowChanged));

    public static int GetRow(DependencyObject obj)
    {
      return (int)obj.GetValue(RowProperty);
    }

    public static void SetRow(DependencyObject obj, int value)
    {
      obj.SetValue(RowProperty, value);
    }

    private static void OnRowChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {

    }

    #endregion

    protected override Size MeasureOverride(Size availableSize)
    {
      var width = availableSize.Width;
      var height = availableSize.Height;
      foreach (UIElement child in InternalChildren)
      {
        if (child == null) { continue; }
        if (GetRow(child) != -1)
        {
          child.Measure(new Size(width, height));
          height -= child.DesiredSize.Height;
        }
        else if (GetColumn(child) != -1)
        {
          child.Measure(new Size(width, height));
          width -= child.DesiredSize.Width;
        }
      }
      var childConstraint = new Size(width / 8, height / 8);
      var src = InternalChildren.Cast<UIElement>().
        Where(c => c != null && 
              GetRow(c) == -1 && 
              GetColumn(c) == -1);
      
      foreach (var child in src)
        child.Measure(childConstraint);

      var maxCellWidth = src.Max(c => c.DesiredSize.Width);
      var maxCellHeight = src.Max(c => c.DesiredSize.Height);

      return new Size(
        Math.Min(maxCellWidth, availableSize.Width), 
        Math.Min(maxCellHeight, availableSize.Height));
    }
    protected override Size ArrangeOverride(Size arrangeSize)
    {
      var w = arrangeSize.Width / 8;
      var h = arrangeSize.Height / 8;

      foreach (UIElement child in InternalChildren)
      {
        if (child == null) { continue; }
        var pos = GetPosition(child);
        child.Arrange(new Rect(
          new Point(pos.X * w, pos.Y * h),
          new Size(w, h)));
      }
      return arrangeSize;
    }
  }
}