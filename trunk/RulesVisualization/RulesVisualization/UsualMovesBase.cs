using System;
using System.Collections.Generic;
using System.Linq;
using Yasc.ShogiCore;

namespace Yasc.RulesVisualization
{
  public abstract class UsualMovesBase : MovesBase, IUsualMoves
  {
    public string From { get; set; }

    Position IUsualMoves.From
    {
      get { return From; }
    }
    IEnumerable<MoveDest> IUsualMoves.To
    {
      get { return IsAvailable ? GetTo() : GetNotTo(); }
    }
    IEnumerable<MoveDest> IUsualMoves.NotTo
    {
      get { return IsAvailable ? GetNotTo() : GetTo(); }
    }

    private IEnumerable<MoveDest> GetTo()
    {
      return from s in To.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries) select new MoveDest(s);
    }
    private IEnumerable<MoveDest> GetNotTo()
    {
      switch (Mode)
      {
        case MovesValidatorMode.AndNoMore:
          var set = new HashSet<MoveDest>(GetTo());
          return from p in Position.OnBoard
                 from i in new[] { false, true }
                 let d = new MoveDest(p, i)
                 where !set.Contains(d)
                 select d;
        default:
          return new MoveDest[0];
      }
    }
  }
}