using System;
using System.Windows.Input;
using Yasc.AI;
using Yasc.Utils.Mvvm;

namespace Yasc.Gui.Game
{
  public class GameWithEngineViewModel : GameWithOpponentViewModel, IDisposable
  {
    public GameWithEngineViewModel()
    {
      InitTicket(new UsiAiController());
      InitBoard();
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