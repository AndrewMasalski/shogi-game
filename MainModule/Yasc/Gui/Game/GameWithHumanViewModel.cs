using Yasc.Networking;

namespace Yasc.Gui.Game
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