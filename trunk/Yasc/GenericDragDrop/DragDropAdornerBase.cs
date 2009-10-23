using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Yasc.GenericDragDrop
{
  /// <summary>Interaction logic for DragDropAdornerBase.xaml</summary>
  public class DragDropAdornerBase : UserControl
  {
    // Using a DependencyProperty as the backing store for DropState.  
    // This enables animation, styling, binding, etc...
    public static readonly DependencyProperty AdornerDropStateProperty =
      DependencyProperty.Register("AdornerDropState", typeof (DropState),
      typeof (DragDropAdornerBase), new UIPropertyMetadata(DropStateChanged));

    public static readonly DependencyProperty SourceVisualProperty =
      DependencyProperty.Register("SourceVisual", typeof(Visual),
      typeof (DragDropAdornerBase));

    public DragDropAdornerBase()
    {
      var transGroup = new TransformGroup();
      
      transGroup.Children.Add(new ScaleTransform(1f, 1f));
      transGroup.Children.Add(new SkewTransform(0f, 0f));
      transGroup.Children.Add(new RotateTransform(0f));
      transGroup.Children.Add(new TranslateTransform(0f, 0f));

      RenderTransform = transGroup;
    }

    public DropState AdornerDropState
    {
      get { return (DropState) GetValue(AdornerDropStateProperty); }
      set { SetValue(AdornerDropStateProperty, value); }
    }
    public Visual SourceVisual
    {
      get { return (Visual)GetValue(SourceVisualProperty); }
      set { SetValue(SourceVisualProperty, value); }
    }

    private static void DropStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DragDropAdornerBase) d).StateChangedHandler(d, e);
    }

    protected virtual void StateChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
    }
  }

  public enum DropState
  {
    CanDrop = 1,
    CannotDrop = 2
  }
}