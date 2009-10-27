using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Yasc.GenericDragDrop
{
  public class DndAdorner : Adorner
  {
    private readonly FrameworkElement _adornedElement;
    private readonly VisualBrush _brush;

    public static readonly DependencyProperty OffsetProperty =
      DependencyProperty.Register("Offset", typeof(Vector), typeof(DndAdorner),
        new FrameworkPropertyMetadata(new Vector(),
          FrameworkPropertyMetadataOptions.AffectsRender));

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
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      drawingContext.DrawGeometry(_brush, null, 
        new RectangleGeometry(
          new Rect((Point)Offset, _adornedElement.RenderSize)
          ));
    }
  }
}