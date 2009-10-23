using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Yasc.GenericDragDrop
{
  public class DragDropHelper
  {
    private static DragDropHelper _instance;
    private DragDropAdornerBase _adorner;
    private Canvas _adornerLayer;
    private Point _delta;
    private object _draggedData;
    private Rect _dropBoundingBox;
    private UIElement _dropTarget;
    private Point _initialMousePosition;
    private bool _mouseCaptured;
    private Point _scrollTarget;
    private Visual _draggedVisual;

    private Window _topWindow;

    private static DragDropHelper Instance
    {
      get
      {
        if (_instance == null)
        {
          _instance = new DragDropHelper();
        }
        return _instance;
      }
    }

    #region Attached Properties

    public static readonly DependencyProperty AdornerLayerProperty =
      DependencyProperty.RegisterAttached("AdornerLayer", typeof (string), 
      typeof (DragDropHelper), new UIPropertyMetadata(null));

    public static readonly DependencyProperty DragDropControlProperty =
      DependencyProperty.RegisterAttached("DragDropControl", typeof (UIElement), 
      typeof (DragDropHelper), new UIPropertyMetadata(null));

    public static readonly DependencyProperty DropTargetProperty =
      DependencyProperty.RegisterAttached("DropTarget", typeof (string), 
      typeof (DragDropHelper), new UIPropertyMetadata(string.Empty));

    public static readonly DependencyProperty IsDragSourceProperty =
      DependencyProperty.RegisterAttached("IsDragSource", typeof (bool), 
      typeof (DragDropHelper), new UIPropertyMetadata(false, IsDragSourceChanged));


    public static bool GetIsDragSource(DependencyObject obj)
    {
      return (bool) obj.GetValue(IsDragSourceProperty);
    }

    public static void SetIsDragSource(DependencyObject obj, bool value)
    {
      obj.SetValue(IsDragSourceProperty, value);
    }

    public static UIElement GetDragDropControl(DependencyObject obj)
    {
      return (UIElement) obj.GetValue(DragDropControlProperty);
    }

    public static void SetDragDropControl(DependencyObject obj, UIElement value)
    {
      obj.SetValue(DragDropControlProperty, value);
    }

    public static string GetDropTarget(DependencyObject obj)
    {
      return (string) obj.GetValue(DropTargetProperty);
    }

    public static void SetDropTarget(DependencyObject obj, string value)
    {
      obj.SetValue(DropTargetProperty, value);
    }

    public static string GetAdornerLayer(DependencyObject obj)
    {
      return (string) obj.GetValue(AdornerLayerProperty);
    }

    public static void SetAdornerLayer(DependencyObject obj, string value)
    {
      obj.SetValue(AdornerLayerProperty, value);
    }


    private static void IsDragSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      var dragSource = obj as UIElement;
      if (dragSource == null) return;
      if (Equals(e.NewValue, true))
      {
        dragSource.PreviewMouseLeftButtonDown += Instance.DragSourcePreviewMouseLeftButtonDown;
        dragSource.PreviewMouseLeftButtonUp += Instance.DragSourcePreviewMouseLeftButtonUp;
        dragSource.PreviewMouseMove += Instance.DragSourcePreviewMouseMove;
      }
      else
      {
        dragSource.PreviewMouseLeftButtonDown -= Instance.DragSourcePreviewMouseLeftButtonDown;
        dragSource.PreviewMouseLeftButtonUp -= Instance.DragSourcePreviewMouseLeftButtonUp;
        dragSource.PreviewMouseMove -= Instance.DragSourcePreviewMouseMove;
      }
    }

    #endregion

    #region Utilities

    public static FrameworkElement FindAncestor(Type ancestorType, Visual visual)
    {
      while (visual != null && !ancestorType.IsInstanceOfType(visual))
      {
        visual = (Visual) VisualTreeHelper.GetParent(visual);
      }
      return visual as FrameworkElement;
    }

    public static bool IsMovementBigEnough(Point initialMousePosition, Point currentPosition)
    {
      return (Math.Abs(currentPosition.X - initialMousePosition.X) >= SystemParameters.MinimumHorizontalDragDistance ||
              Math.Abs(currentPosition.Y - initialMousePosition.Y) >= SystemParameters.MinimumVerticalDragDistance);
    }

    #endregion

    #region Drag Handlers

    private void DragSourcePreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      try
      {
        var visual = e.OriginalSource as Visual;
        _topWindow = (Window) FindAncestor(typeof (Window), visual);
        _initialMousePosition = e.GetPosition(_topWindow);

        string adornerLayerName = GetAdornerLayer(sender as DependencyObject);
        _adornerLayer = (Canvas) _topWindow.FindName(adornerLayerName);

        string dropTargetName = GetDropTarget(sender as DependencyObject);
        _dropTarget = (UIElement) _topWindow.FindName(dropTargetName);

        _draggedData = sender;
        _draggedVisual = (Visual) sender;
      }
      catch (Exception exc)
      {
        Console.WriteLine("Exception in DragDropHelper: " + exc.InnerException);
      }
    }

    // Drag = mouse down + move by a certain amount
    private void DragSourcePreviewMouseMove(object sender, MouseEventArgs e)
    {
      if (!_mouseCaptured && _draggedData != null)
      {
        // Only drag when user moved the mouse by a reasonable amount.
        if (IsMovementBigEnough(_initialMousePosition, e.GetPosition(_topWindow)))
        {
          _adorner = (DragDropAdornerBase) GetDragDropControl(sender as DependencyObject);
          _adorner.DataContext = _draggedData;
//          _adorner.SourceVisual = _draggedVisual;
          _adorner.Opacity = 0.7;

          _adornerLayer.Visibility = Visibility.Visible;
          _adornerLayer.Children.Add(_adorner);
          _mouseCaptured = Mouse.Capture(_adorner);

          Canvas.SetLeft(_adorner, _initialMousePosition.X - 20);
          Canvas.SetTop(_adorner, _initialMousePosition.Y - 15);
          _adornerLayer.PreviewMouseMove += AdornerMouseMove;
          _adornerLayer.PreviewMouseUp += AdornerMouseUp;
        }
      }
    }

    private void AdornerMouseMove(object sender, MouseEventArgs e)
    {
      Point currentPoint = e.GetPosition(_topWindow);
      currentPoint.X = currentPoint.X - 20;
      currentPoint.Y = currentPoint.Y - 15;

      _delta = new Point(_initialMousePosition.X - currentPoint.X, _initialMousePosition.Y - currentPoint.Y);
      _scrollTarget = new Point(_initialMousePosition.X - _delta.X, _initialMousePosition.Y - _delta.Y);

      Canvas.SetLeft(_adorner, _scrollTarget.X);
      Canvas.SetTop(_adorner, _scrollTarget.Y);

      _adorner.AdornerDropState = DropState.CannotDrop;

      if (_dropTarget != null)
      {
        GeneralTransform t = _dropTarget.TransformToVisual(_adornerLayer);
        _dropBoundingBox = t.TransformBounds(new Rect(0, 0, _dropTarget.RenderSize.Width, _dropTarget.RenderSize.Height));

        if (e.GetPosition(_adornerLayer).X > _dropBoundingBox.Left &&
            e.GetPosition(_adornerLayer).X < _dropBoundingBox.Right &&
            e.GetPosition(_adornerLayer).Y > _dropBoundingBox.Top &&
            e.GetPosition(_adornerLayer).Y < _dropBoundingBox.Bottom)
        {
          _adorner.AdornerDropState = DropState.CanDrop;
        }
      }
    }

    private void AdornerMouseUp(object sender, MouseEventArgs e)
    {
      switch (_adorner.AdornerDropState)
      {
        case DropState.CanDrop:
          try
          {
            var storyboard = ((Storyboard) _adorner.Resources["canDrop"]);
            storyboard.Completed += (s, args) =>
                {
                  _adornerLayer.Children.Clear();
                  _adornerLayer.Visibility = Visibility.Collapsed;
                };
            storyboard.Begin(_adorner);

            if (ItemDropped != null)
              ItemDropped(_adorner, new DragDropEventArgs(_draggedData));
          }
          catch (Exception x)
          {
            Console.WriteLine(x);
          }
          break;
        case DropState.CannotDrop:
          try
          {
            var sb = (Storyboard)_adorner.Resources["cannotDrop"];
            var aniX = (DoubleAnimation)sb.Children[0];
            aniX.To = _delta.X;
            var aniY = (DoubleAnimation)sb.Children[1];
            aniY.To = _delta.Y;
            sb.Completed += (s, args) =>
                              {
                                _adornerLayer.Children.Clear();
                                _adornerLayer.Visibility = Visibility.Collapsed;
                              };
            sb.Begin(_adorner);
          }
          catch (Exception x)
          {
            Console.WriteLine(x);
          }
          break;
      }

      _draggedData = null;
      _draggedVisual = null;
      _adornerLayer.PreviewMouseMove -= AdornerMouseMove;
      _adornerLayer.PreviewMouseUp -= AdornerMouseUp;

      if (_adorner != null)
      {
        _adorner.ReleaseMouseCapture();
      }
      _adorner = null;
      _mouseCaptured = false;
    }

    private void DragSourcePreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      _draggedData = null;
      _draggedVisual = null;
      _mouseCaptured = false;

      if (_adorner != null)
      {
        _adorner.ReleaseMouseCapture();
      }
    }

    #endregion

    #region Events

    public static event EventHandler<DragDropEventArgs> ItemDropped;

    #endregion
  }

  public class DragDropEventArgs : EventArgs
  {
    public object Content;

    public DragDropEventArgs()
    {
    }

    public DragDropEventArgs(object content)
    {
      Content = content;
    }
  }
}