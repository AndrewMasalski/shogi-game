using System.Windows;
using Yasc.ShogiCore.Moves;

namespace Yasc.BoardControl.Controls
{
  public class MoveAttemptEventArgs : RoutedEventArgs
  {
    public MoveBase Move { get; private set; }

    public MoveAttemptEventArgs(RoutedEvent routedEvent, object source, MoveBase move) 
      : base(routedEvent, source)
    {
      Move = move;
    }
  }
}