using System.Windows;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.MovesHistory;

namespace Yasc.BoardControl.Controls
{
  public class MoveAttemptEventArgs : RoutedEventArgs
  {
    public DecoratedMove DecoratedMove { get; private set; }

    public MoveAttemptEventArgs(RoutedEvent routedEvent, object source, DecoratedMove move) 
      : base(routedEvent, source)
    {
      DecoratedMove = move;
    }
  }
}