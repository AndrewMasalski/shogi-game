using System.Collections;
using System.Collections.Generic;
using Yasc.ShogiCore.Moves;
using Yasc.ShogiCore.Utils;

namespace Yasc.ShogiCore
{
  public class AvailableDropMovesAnalyser : IEnumerable<DropMoveSnapshot>
  {
    private readonly BoardSnapshot _snapshot;
    private readonly PieceSnapshot _pieceSnapshot;

    public AvailableDropMovesAnalyser(BoardSnapshot snapshot, PieceSnapshot pieceSnapshot)
    {
      _snapshot = snapshot;
      _pieceSnapshot = pieceSnapshot;
    }

    public IEnumerator<DropMoveSnapshot> GetEnumerator()
    {
      foreach (var p in Position.OnBoard)
        if (_snapshot[p] == null)
          yield return new DropMoveSnapshot(_pieceSnapshot, p);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}