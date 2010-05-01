using System;
using System.Windows.Input;
using Yasc.AI;
using Yasc.Networking.Interfaces;
using Yasc.Utils.Mvvm;

namespace Yasc.Gui.Game
{
  public class GameWithEngineViewModel : GameWithOpponentViewModel, IDisposable
  {
    private GameWithEngineViewModel(IPlayerGameController controller)
    {
      InitTicket(controller);
      InitBoard();
    }

    public static GameWithEngineViewModel Create()
    {
      var controller = UsiAiController.Create();
      return controller == null ? null :
        new GameWithEngineViewModel(controller);
    }
    private RelayCommand _openEngineSettingsCommand;

    public ICommand OpenEngineSettingsCommand
    {
      get { return _openEngineSettingsCommand ?? (_openEngineSettingsCommand = new RelayCommand(OpenEngineSettings)); }
    }

    private void OpenEngineSettings()
    {
      

    }


    public void Dispose()
    {
      if (Ticket != null)
      {
        Ticket.Dispose();
      }
    }

  }
}