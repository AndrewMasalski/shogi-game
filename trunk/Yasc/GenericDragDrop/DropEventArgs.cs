using System.Windows;

namespace Yasc.GenericDragDrop
{
  public class DropEventArgs : RoutedEventArgs
  {
    public FrameworkElement DragSource { get; private set; }
    public FrameworkElement DragTarget { get; private set; }

    public DropEventArgs(RoutedEvent routedEvent, object source, FrameworkElement dragSource, FrameworkElement dragTarget)
      : base(routedEvent, source)
    {
      DragSource = dragSource;
      DragTarget = dragTarget;
    }
  }
}