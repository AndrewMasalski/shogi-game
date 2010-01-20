using MvvmFoundation.Wpf;
using Yasc.Networking;

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