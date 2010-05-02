using Yasc.Networking.Interfaces;

namespace MainModule.Gui.Game
{
  public class GameWithHumanViewModel : GameWithOpponentViewModel
  {
    public GameWithHumanViewModel(IPlayerGameController ticket)
    {
      InitTicket(ticket);
      InitBoard();
    }
  }
}