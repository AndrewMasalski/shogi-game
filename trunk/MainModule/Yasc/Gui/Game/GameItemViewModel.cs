using Yasc.Networking.Interfaces;
using Yasc.Utils.Mvvm;

namespace Yasc.Gui.Game
{
  public class GameItemViewModel : ObservableObject
  {
    public IServerGame ServerGame { get; set; }

    public GameItemViewModel(IServerGame serverGame)
    {
      ServerGame = serverGame;
    }
  }
}