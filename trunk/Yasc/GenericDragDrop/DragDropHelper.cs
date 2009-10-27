using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Yasc.GenericDragDrop
{
  public class DragDropHelper : IDisposable
  {
    private readonly UIElement _dragSource;
    private MyAdorner _adorner;
    private Point _initialMousePosition;
    private bool _mouseCaptured;

    private Window _topWindow;

    #region Attached Properties

    public static readonly DependencyProperty DropTargetProperty =
      DependencyProperty.RegisterAttached("DropTarget", typeof(string),
      typeof(DragDropHelper), new UIPropertyMetadata(string.Empty));

    public static readonly DependencyProperty IsDragSourceProperty =
      DependencyProperty.RegisterAttached("IsDragSource", typeof(bool),
      typeof(DragDropHelper), new UIPropertyMetadata(false, IsDragSourceChanged));

    private static readonly DependencyPropertyKey DragDropHelperPropertyKey =
      DependencyProperty.RegisterAttachedReadOnly("DragDropHelper", typeof(DragDropHelper),
      typeof(DragDropHelper), new UIPropertyMetadata(null));

    public static readonly DependencyProperty DragDropHelperProperty = DragDropHelperPropertyKey.DependencyProperty;
    private bool _started;
    private AdornerLayer _adornerLayer;

    private DragDropHelper(UIElement dragSource)
    {
      _dragSource = dragSource;
      _dragSource.PreviewMouseLeftButtonDown += DragSourcePreviewMouseDown;
      _dragSource.PreviewMouseLeftButtonUp += DragSourcePreviewMouseLeftButtonUp;
      _dragSource.PreviewMouseMove += DragSourcePreviewMouseMove;
    }

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

    private static void IsDragSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      var dragSource = obj as UIElement;

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

    #endregion


    #region Drag Handlers

    private void DragSourcePreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      _topWindow = _dragSource.FindAncestor<Window>();
      _initialMousePosition = e.GetPosition(_topWindow);
      _started = true;
    }

    private void DragSourcePreviewMouseMove(object sender, MouseEventArgs e)
    {
      if (!_started || _mouseCaptured) return;

      _adornerLayer = AdornerLayer.GetAdornerLayer((Visual)sender);
      _adorner = new MyAdorner((FrameworkElement)sender);
      _adornerLayer.Add(_adorner);

      _mouseCaptured = _adorner.CaptureMouse();

      _topWindow.PreviewMouseMove += AdornerMouseMove;
      _topWindow.PreviewMouseUp += AdornerMouseUp;
    }

    private void AdornerMouseMove(object sender, MouseEventArgs e)
    {
      _adorner.Offset = e.GetPosition(_topWindow) - _initialMousePosition;
    }

    private void AdornerMouseUp(object sender, MouseEventArgs e)
    {
      _topWindow.PreviewMouseMove -= AdornerMouseMove;
      _topWindow.PreviewMouseUp -= AdornerMouseUp;

      Release();
    }

    private void DragSourcePreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Release();
    }

    private void Release()
    {
      _mouseCaptured = false;
      _started = false;

      if (_adorner != null)
      {
        _adorner.ReleaseMouseCapture();
        _adornerLayer.Remove(_adorner);
      }
      _adorner = null;
    }

    #endregion

    #region Implementation of IDisposable

    public void Dispose()
    {
      _dragSource.PreviewMouseLeftButtonDown -= DragSourcePreviewMouseDown;
      _dragSource.PreviewMouseLeftButtonUp -= DragSourcePreviewMouseLeftButtonUp;
      _dragSource.PreviewMouseMove -= DragSourcePreviewMouseMove;
    }

    #endregion
  }
}