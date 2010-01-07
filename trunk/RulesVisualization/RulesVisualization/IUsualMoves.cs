using System.Collections.Generic;
using Yasc.ShogiCore;

namespace Yasc.RulesVisualization
{
  public interface IUsualMoves
  {
    Position From { get; }
    IEnumerable<MoveDest> To { get; }
    IEnumerable<MoveDest> NotTo { get; }
    bool IsExclusive { get; }
  }
}