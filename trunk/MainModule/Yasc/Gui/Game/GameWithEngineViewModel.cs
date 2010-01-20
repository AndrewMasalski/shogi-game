using System;
using System.Windows.Input;
using MvvmFoundation.Wpf;
using Yasc.AI;

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
      get
      {
        if (_openEngineSettingsCommand == null)
        {
          _openEngineSettingsCommand = new RelayCommand(OpenEngineSettings);
        }
        return _openEngineSettingsCommand;
      }
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