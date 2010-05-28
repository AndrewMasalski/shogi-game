using System.Windows;
using Yasc.ShogiCore.Snapshots;

namespace Yasc.BoardControl.Controls
{
  public class MoveAttemptEventArgs : RoutedEventArgs
  {
    public Move Move { get; private set; }

    public MoveAttemptEventArgs(RoutedEvent routedEvent, object source, Move move) 
      : base(routedEvent, source)
    {
      Move = move;
    }
  }
}