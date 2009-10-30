using System;
using System.Collections.Generic;
using Yasc.ShogiCore.Moves;

namespace Yasc.ShogiCore
{
  public class PromotionAnalyser
  {
    enum PromotionOption
    {
      CannotBePromoted, CanBePromoted, MustBePromoted
    }

    public static IEnumerable<UsualMoveSnapshot> DuplicateForPromoting(IEnumerable<UsualMoveSnapshot> enumerable)
    {
      foreach (var snapshot in enumerable)
      {
        switch (AnalysePromotion(snapshot))
        {
          case PromotionOption.CannotBePromoted:
            yield return snapshot;
            break;
          case PromotionOption.CanBePromoted:
            yield return snapshot;
            yield return Promoted(snapshot);
            break;
          case PromotionOption.MustBePromoted:
            yield return Promoted(snapshot);
            break;
        }
      }
    }


    private static UsualMoveSnapshot Promoted(UsualMoveSnapshot snapshot)
    {
      throw new NotImplementedException();
    }

    private static PromotionOption AnalysePromotion(UsualMoveSnapshot moveSnapshot)
    {
      throw new NotImplementedException();
    }
    
  }
}