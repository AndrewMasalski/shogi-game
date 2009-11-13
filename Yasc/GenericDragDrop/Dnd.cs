using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace Yasc.GenericDragDrop
{
  public class Dnd : IDisposable
  {
    #region ' Static Stuff '

    public static readonly DependencyProperty IsDropTargetProperty =
      DependencyProperty.RegisterAttached("IsDropTarget", typeof(bool),
      typeof(Dnd), new UIPropertyMetadata(false));

    public static readonly DependencyProperty IsDragSourceProperty =
      DependencyProperty.RegisterAttached("IsDragSource", typeof(bool),
      typeof(Dnd), new UIPropertyMetadata(false, IsDragSourceChanged));

    private static readonly DependencyPropertyKey _dragDropHelperPropertyKey =
      DependencyProperty.RegisterAttachedReadOnly("Dnd", typeof(Dnd),
      typeof(Dnd), new UIPropertyMetadata(null));

    private static void IsDragSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      var dragSource = obj as FrameworkElement;

      if (dragSource == null)
        throw new NotSupportedException(
          "You can't set Dnd.IsDragSource if object is not UIElement ");

      var value = (bool)e.NewValue;
      var helper = GetDragDropHelper(dragSource);
      if (value && helper == null)
      {
        dragSource.SetValue(_dragDropHelperPropertyKey, new Dnd(dragSource));
      }
      else if (!value && helper != null)
      {
        helper.Dispose();
        dragSource.ClearValue(_dragDropHelperPropertyKey);
      }
    }

    public static readonly DependencyProperty DragDropHelperProperty = _dragDropHelperPropertyKey.DependencyProperty;
    public static Dnd GetDragDropHelper(DependencyObject obj)
    {
      return (Dnd)obj.GetValue(DragDropHelperProperty);
    }
    public static bool GetIsDragSource(DependencyObject obj)
    {
      return (bool)obj.GetValue(IsDragSourceProperty);
    }
    public static void SetIsDragSource(DependencyObject obj, bool value)
    {
      obj.SetValue(IsDragSourceProperty, value);
    }
    public static bool GetIsDropTarget(DependencyObject obj)
    {
      return (bool)obj.GetValue(IsDropTargetProperty);
    }
    public static void SetIsDropTarget(DependencyObject obj, bool value)
    {
      obj.SetValue(IsDropTargetProperty, value);
    }

    public static readonly RoutedEvent DropEvent = EventManager.RegisterRoutedEvent(
        "Drop", RoutingStrategy.Bubble, typeof(EventHandler<DropEventArgs>), typeof(Dnd));

    public static void AddDropHandler(DependencyObject d, EventHandler<DropEventArgs> handler)
    {
      ((UIElement)d).AddHandler(DropEvent, handler);
    }
    public static void RemoveDropHandler(DependencyObject d, EventHandler<DropEventArgs> handler)
    {
      ((UIElement)d).RemoveHandler(DropEvent, handler);
    }
    private static void RaiseDropEvent(FrameworkElement d, FrameworkElement source)
    {
      source.RaiseEvent(new DropEventArgs(DropEvent, source, source, d));
    }
    public static readonly RoutedEvent DragEvent = EventManager.RegisterRoutedEvent(
        "Drag", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Dnd));

    public static void AddDragHandler(DependencyObject d, RoutedEventHandler handler)
    {
      ((UIElement)d).AddHandler(DragEvent, handler);
    }
    public static void RemoveDragHandler(DependencyObject d, RoutedEventHandler handler)
    {
      ((UIElement)d).RemoveHandler(DragEvent, handler);
    }
    private static void RaiseDragEvent(IInputElement d)
    {
      d.RaiseEvent(new RoutedEventArgs(DragEvent, d));
    }

    #endregion

    private Dnd(FrameworkElement dragSource)
    {
      _dragSource = dragSource;
      _dragSource.PreviewMouseLeftButtonDown += MouseDown;
    }

    private DndAdorner Adorner
    {
      get
      {
        if (_adorner == null)
        {
          _adorner = new DndAdorner(_dragSource) { IsHitTestVisible = false };
        }
        return _adorner;
      }
    }
    private AdornerLayer AdornerLayer
    {
      get
      {
        if (_adornerLayer == null)
        {
          _adornerLayer = AdornerLayer.GetAdornerLayer(_dragSource);
        }
        return _adornerLayer;
      }
    }

    private void MouseDown(object sender, MouseButtonEventArgs e)
    {
      _topWindow = _dragSource.FindAncestor<Window>();
      _initialMousePosition = e.GetPosition(_topWindow);

      _topWindow.PreviewMouseMove += MouseMove;
      _topWindow.PreviewMouseUp += MouseUp;
      RaiseDragEvent(_dragSource);
    }
    private void MouseMove(object sender, MouseEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Released)
      {
        Release();
        return;
      }
      if (!_adornerIsShown)
      {
        AdornerLayer.Add(Adorner);
        _adornerIsShown = true;
      }

      Adorner.Offset = e.GetPosition(_topWindow) - _initialMousePosition;
    }
    private void MouseUp(object sender, MouseEventArgs e)
    {
      var target = ((DependencyObject)e.OriginalSource).
        FindAncestor<FrameworkElement>(GetIsDropTarget);
      RaiseDropEvent(target, _dragSource);
      Release();
    }
    private void Release()
    {
      _topWindow.PreviewMouseMove -= MouseMove;
      _topWindow.PreviewMouseUp -= MouseUp;
      if (_adornerIsShown)
      {
        _adornerLayer.Remove(_adorner);
        _adornerIsShown = false;
      }
    }

    public void Dispose()
    {
      _dragSource.PreviewMouseLeftButtonDown -= MouseDown;
    }

    #region ' Fields '

    private DndAdorner _adorner;
    private AdornerLayer _adornerLayer;
    private Window _topWindow;

    private Point _initialMousePosition;
    private readonly FrameworkElement _dragSource;
    private bool _adornerIsShown;

    #endregion
  }
}