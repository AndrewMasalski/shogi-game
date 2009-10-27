using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace Yasc.GenericDragDrop
{
  public class DragDropHelper : IDisposable
  {
    #region ' Static Stuff '

    public static readonly DependencyProperty DropTargetProperty =
      DependencyProperty.RegisterAttached("DropTarget", typeof(string),
      typeof(DragDropHelper), new UIPropertyMetadata(string.Empty));

    public static readonly DependencyProperty IsDragSourceProperty =
      DependencyProperty.RegisterAttached("IsDragSource", typeof(bool),
      typeof(DragDropHelper), new UIPropertyMetadata(false, IsDragSourceChanged));

    private static readonly DependencyPropertyKey DragDropHelperPropertyKey =
      DependencyProperty.RegisterAttachedReadOnly("DragDropHelper", typeof(DragDropHelper),
      typeof(DragDropHelper), new UIPropertyMetadata(null));

    private static void IsDragSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      var dragSource = obj as FrameworkElement;

      if (dragSource == null)
        throw new NotSupportedException(
          "You can't set DragDropHelper.IsDragSource if object is not UIElement ");

      var value = (bool)e.NewValue;
      var helper = GetDragDropHelper(dragSource);
      if (value && helper == null)
      {
        dragSource.SetValue(DragDropHelperPropertyKey, new DragDropHelper(dragSource));
      }
      else if (!value && helper != null)
      {
        helper.Dispose();
        dragSource.ClearValue(DragDropHelperPropertyKey);
      }
    }

    public static readonly DependencyProperty DragDropHelperProperty = DragDropHelperPropertyKey.DependencyProperty;
    public static DragDropHelper GetDragDropHelper(DependencyObject obj)
    {
      return (DragDropHelper)obj.GetValue(DragDropHelperProperty);
    }
    public static bool GetIsDragSource(DependencyObject obj)
    {
      return (bool)obj.GetValue(IsDragSourceProperty);
    }
    public static void SetIsDragSource(DependencyObject obj, bool value)
    {
      obj.SetValue(IsDragSourceProperty, value);
    }
    public static string GetDropTarget(DependencyObject obj)
    {
      return (string)obj.GetValue(DropTargetProperty);
    }
    public static void SetDropTarget(DependencyObject obj, string value)
    {
      obj.SetValue(DropTargetProperty, value);
    }

    public static readonly RoutedEvent DropEvent = EventManager.RegisterRoutedEvent(
        "Drop", RoutingStrategy.Bubble, typeof(EventHandler<DropEventArgs>), typeof(DragDropHelper));

    public static void AddDropHandler(DependencyObject d, EventHandler<DropEventArgs> handler)
    {
      ((UIElement)d).AddHandler(DropEvent, handler);
    }
    public static void RemoveDropHandler(DependencyObject d, EventHandler<DropEventArgs> handler)
    {
      ((UIElement)d).RemoveHandler(DropEvent, handler);
    }
    private static void RaiseDropEvent(DependencyObject d, object source)
    {
      ((UIElement)d).RaiseEvent(new DropEventArgs(DropEvent, 
        source, (FrameworkElement)source, (FrameworkElement)d));
    }

    #endregion

    private DragDropHelper(FrameworkElement dragSource)
    {
      _dragSource = dragSource;
      _dragSource.PreviewMouseLeftButtonDown += MouseDown;
    }

    private MyAdorner Adorner
    {
      get
      {
        if (_adorner == null)
        {
          _adornerLayer = AdornerLayer.GetAdornerLayer(_dragSource);
          _adorner = new MyAdorner(_dragSource) { IsHitTestVisible = false };
          _adornerLayer.Add(_adorner);
        }
        return _adorner;
      }
    }

    private void MouseDown(object sender, MouseButtonEventArgs e)
    {
      _topWindow = _dragSource.FindAncestor<Window>();
      _initialMousePosition = e.GetPosition(_topWindow);

      _topWindow.PreviewMouseMove += MouseMove;
      _topWindow.PreviewMouseUp += MouseUp;
    }
    private void MouseMove(object sender, MouseEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Released)
      {
        Release();
        return;
      }
      Adorner.Offset = e.GetPosition(_topWindow) - _initialMousePosition;
    }
    private void MouseUp(object sender, MouseEventArgs e)
    {
      var target = ((DependencyObject) e.OriginalSource).
        FindAncestor<FrameworkElement>(a => GetDropTarget(a) == "ShogiBoardGrid");
      if (target != null)
        RaiseDropEvent(target, _dragSource);
      Release();
    }
    private void Release()
    {
      _topWindow.PreviewMouseMove -= MouseMove;
      _topWindow.PreviewMouseUp -= MouseUp;
      _adornerLayer.Remove(_adorner);
    }

    public void Dispose()
    {
      _dragSource.PreviewMouseLeftButtonDown -= MouseDown;
    }

    #region ' Fields '

    private MyAdorner _adorner;
    private AdornerLayer _adornerLayer;
    private Window _topWindow;

    private Point _initialMousePosition;
    private readonly FrameworkElement _dragSource;

    #endregion
  }
}