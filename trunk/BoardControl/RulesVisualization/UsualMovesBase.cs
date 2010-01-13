using System;
using System.Collections.Generic;
using System.Linq;
using Yasc.Controls;
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
      return Mode == MovesValidatorMode.AndNoMore ? 
        GetComplement(new HashSet<MoveDest>(GetTo())) : new MoveDest[0];
    }

    private static IEnumerable<MoveDest> GetComplement(ICollection<MoveDest> set)
    {
      return from position in Position.OnBoard
             from promotion in new[] { false, true }
             let dest = new MoveDest(position, promotion)
             where !set.Contains(dest)
             select dest;
    }

    public override void ShowMoves(ShogiBoard board)
    {
      var usualMoves = (IUsualMoves)this;

      board.GetCell(usualMoves.From).IsMoveSource = true;

      foreach (var p in usualMoves.To)
        board.GetCell(p.Position).IsPossibleMoveTarget = true;
    }
  }
}