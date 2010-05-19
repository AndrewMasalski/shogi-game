using System.Collections.Generic;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Primitives;

namespace Yasc.RulesVisualization
{
  public interface IDropMoves
  {
    PieceColor For { get; }
    PieceType Piece { get; }
    IEnumerable<Position> To { get; }
    IEnumerable<Position> NotTo { get; }
    bool IsExclusive { get;  }
  }
}