using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Yasc.BoardControl.GenericDragDrop
{
  /// <summary>Dnd = Drag and Drop</summary>
  public class DndAdorner : Adorner
  {
    private readonly VisualBrush _brush;
    private readonly RectangleGeometry _geometry;
    private readonly FrameworkElement _adornedElement;

    public static readonly DependencyProperty OffsetProperty =
      DependencyProperty.Register("Offset", typeof(Vector), typeof(DndAdorner),
        new UIPropertyMetadata(new Vector(), OnOffsetChanged));

    private static void OnOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
    {
      ((DndAdorner) o).OnOffsetChanged((Vector) args.NewValue);
    }

    private void OnOffsetChanged(Vector offset)
    {
      _geometry.Rect = new Rect((Point)offset, _adornedElement.RenderSize);
    }


    public Vector Offset
    {
      get { return (Vector)GetValue(OffsetProperty); }
      set { SetValue(OffsetProperty, value); }
    }

    public DndAdorner(FrameworkElement adornedElement)
      : base(adornedElement)
    {
      _adornedElement = adornedElement;
      _brush = new VisualBrush(adornedElement);
      _geometry = new RectangleGeometry(
        new Rect((Point)Offset, _adornedElement.RenderSize));
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      drawingContext.DrawGeometry(_brush, null, _geometry);
    }
  }
}