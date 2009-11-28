using System;
using System.Collections.Generic;
using System.Linq;
using Yasc.ShogiCore.Utils;

namespace Yasc.RulesVisualization
{
  public abstract class MovesBase
  {
    public string To { get; set; }
    public MovesValidatorMode Mode { get; set; }
    public abstract bool IsAvailable { get; }

    protected IEnumerable<Position> GetTo()
    {
      return from s in To.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries) select (Position)s;
    }
    protected IEnumerable<Position> GetNotTo()
    {
      switch (Mode)
      {
        case MovesValidatorMode.AndNoMore:
          var set = new HashSet<Position>(GetTo());
          return from p in Position.OnBoard where !set.Contains(p) select p;
        default:
          return new Position[0];
      }
    }

    public bool IsExclusive
    {
      get { return Mode == MovesValidatorMode.AndNoMore; }
    }
  }
}