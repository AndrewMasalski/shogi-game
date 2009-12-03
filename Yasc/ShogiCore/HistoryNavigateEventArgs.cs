using System;
using Yasc.ShogiCore.SnapShots;

namespace Yasc.ShogiCore
{
  public class HistoryNavigateEventArgs : EventArgs
  {
    public int Step { get; private set; }
    public BoardSnapshot Snapshot { get; private set; }

    public HistoryNavigateEventArgs(int step, BoardSnapshot snapshot)
    {
      Step = step;
      Snapshot = snapshot;
    }
  }
}