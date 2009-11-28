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

    public bool IsExclusive
    {
      get { return Mode == MovesValidatorMode.AndNoMore; }
    }
  }
}