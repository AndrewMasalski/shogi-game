using System.Collections.Generic;
using Yasc.ShogiCore.Moves;
using Yasc.ShogiCore.Moves.Validation;

namespace Yasc.ShogiCore
{
  public class PromotionAnalyser
  {
    public static IEnumerable<UsualMoveSnapshot> DuplicateForPromoting(BoardSnapshot board, IEnumerable<UsualMoveSnapshot> moves)
    {
      foreach (var m in moves)
      {
        var allowed = UsualMovesValidator.
          IsPromotionAllowed(board[m.From], m.From, m.To) == null;

        var mandatory = UsualMovesValidator.
          IsPromotionAllowed(board[m.From], m.From, m.To) == null;

        if (!mandatory) yield return m;
        if (allowed) yield return Promoted(m);
      }
    }

    private static UsualMoveSnapshot Promoted(UsualMoveSnapshot m)
    {
      return new UsualMoveSnapshot(m.From, m.To, !m.IsPromoting);
    }
  }
}