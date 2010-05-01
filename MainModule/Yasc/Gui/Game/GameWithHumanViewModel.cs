using Yasc.Networking;
using Yasc.Networking.Interfaces;

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