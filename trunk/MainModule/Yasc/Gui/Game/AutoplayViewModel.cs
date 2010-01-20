namespace Yasc.Gui.Game
{
  public class AutoplayViewModel : GameViewModel
  {
    public AutoplayViewModel()
    {
      InitBoard();
      IsItMyMove = true;
    }
  }
}