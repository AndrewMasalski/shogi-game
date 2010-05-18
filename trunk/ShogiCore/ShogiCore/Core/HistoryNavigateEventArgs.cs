using System;
using Yasc.ShogiCore.Snapshots;

namespace Yasc.ShogiCore
{
  /// <summary>Contains info about navigation in the moves history</summary>
  public class HistoryNavigateEventArgs : EventArgs
  {
    /// <summary>Indicates how many moves forward user navigates.
    ///   Positive or negative depending on direction of navigation.</summary>
    public int Step { get; private set; }
    /// <summary>Snapshot of the state in histiry user is being navigating to</summary>
    public BoardSnapshot Snapshot { get; private set; }

    internal HistoryNavigateEventArgs(int step, BoardSnapshot snapshot)
    {
      Step = step;
      Snapshot = snapshot;
    }
  }
}