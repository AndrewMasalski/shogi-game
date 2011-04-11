using System.Windows;
using System.Windows.Controls;

namespace Chess
{
  public class BoardPanel : Panel
  {
    #region ' Position : Position Attached Property '

    public static readonly DependencyProperty PositionProperty = DependencyProperty.
      RegisterAttached("Position", typeof (Position), typeof (BoardPanel),
                       new FrameworkPropertyMetadata(default(Position), OnPositionChanged));

    public static Position GetPosition(DependencyObject obj)
    {
      return (Position) obj.GetValue(PositionProperty);
    }

    public static void SetPosition(DependencyObject obj, Position value)
    {
      obj.SetValue(PositionProperty, value);
    }

    private static void OnPositionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {

    }

    #endregion

    protected override Size MeasureOverride(Size availableSize)
    {
      var childConstraint = new Size(availableSize.Width / 8, availableSize.Height / 8);

      foreach (UIElement child in InternalChildren)
      {
        if (child == null) { continue; }
        child.Measure(childConstraint);
      }

      return new Size();
    }
    protected override Size ArrangeOverride(Size arrangeSize)
    {
      foreach (UIElement child in InternalChildren)
      {
        if (child == null) { continue; }

        double x = 0;
        double y = 0;


        //Compute offset of the child:
        //If Left is specified, then Right is ignored
        //If Left is not specified, then Right is used
        //If both are not there, then 0 
        double left = GetPosition(child);
        if (!DoubleUtil.IsNaN(left))
        {
          x = left;
        }
        else
        {
          double right = GetRight(child);

          if (!DoubleUtil.IsNaN(right))
          {
            x = arrangeSize.Width - child.DesiredSize.Width - right;
          }
        }

        double top = GetTop(child);
        if (!DoubleUtil.IsNaN(top))
        {
          y = top;
        }
        else
        {
          double bottom = GetBottom(child);

          if (!DoubleUtil.IsNaN(bottom))
          {
            y = arrangeSize.Height - child.DesiredSize.Height - bottom;
          }
        }

        child.Arrange(new Rect(new Point(x, y), child.DesiredSize));
      }
      return arrangeSize;
    }
  }
}