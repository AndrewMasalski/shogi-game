using System.Collections.Generic;
using Yasc.ShogiCore.Utils;

namespace Yasc.RulesVisualization
{
  public interface IUsualMoves
  {
    Position From { get; }
    IEnumerable<Position> To { get; }
    IEnumerable<Position> NotTo { get; }
    bool IsExclusive { get; }
  }
}