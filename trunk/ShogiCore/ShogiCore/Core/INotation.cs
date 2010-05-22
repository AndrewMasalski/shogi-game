using System.Collections.Generic;
using Yasc.ShogiCore.Snapshots;

namespace Yasc.ShogiCore.Core
{
  /// <summary>Describes one of the ways moves can be transcribed</summary>
  public interface INotation
  {
    /// <summary>Gets move on the board parsing it from transcript</summary>
    /// <param name="originalBoardState">State of the board before move</param>
    /// <param name="move">Move trancsript to parse</param>
    /// <returns>All moves which may be transcribed given way. 
    ///   Doesn't return null but be prepared to receive 0 moves.</returns>
    IEnumerable<MoveSnapshotBase> Parse(BoardSnapshot originalBoardState, string move);

    /// <summary>Returns the transcript for a given move</summary>
    /// <param name="originalBoardState">State of the board before move</param>
    /// <param name="move">Move to trancsript</param>
    string ToString(BoardSnapshot originalBoardState, MoveSnapshotBase move);
  }
}