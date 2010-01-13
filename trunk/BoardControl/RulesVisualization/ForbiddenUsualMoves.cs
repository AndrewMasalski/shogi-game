namespace Yasc.RulesVisualization
{
  public class ForbiddenUsualMoves : UsualMovesBase
  {
    public override bool IsAvailable
    {
      get { return false; }
    }
  }
}