using Yasc.BoardControl.Controls;

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

    public abstract void ShowMoves(ShogiBoard board);
  }
}