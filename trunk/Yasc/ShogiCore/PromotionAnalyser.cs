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
          IsPromotionMandatory(board[m.From], m.To) != null;

        if (!mandatory) yield return new UsualMoveSnapshot(m.From, m.To, false);
        if (allowed) yield return new UsualMoveSnapshot(m.From, m.To, true);
      }
    }
  }
}