using System.Collections.Generic;
using Yasc.ShogiCore.Utils;

namespace Yasc.RulesVisualization
{
  public abstract class UsualMovesBase : MovesBase, IUsualMoves
  {
    public string From { get; set; }

    Position IUsualMoves.From
    {
      get { return From; }
    }
    IEnumerable<Position> IUsualMoves.To
    {
      get { return IsAvailable ? GetTo() : GetNotTo(); }
    }
    IEnumerable<Position> IUsualMoves.NotTo
    {
      get { return IsAvailable ? GetNotTo() : GetTo(); }
    }
  }
}