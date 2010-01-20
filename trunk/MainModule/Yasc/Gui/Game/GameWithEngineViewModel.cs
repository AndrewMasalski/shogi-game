using System;
using Yasc.AI;

namespace Yasc.Gui.Game
{
  public class GameWithEngineViewModel : GameWithOpponentViewModel, IDisposable
  {
    public GameWithEngineViewModel()
    {
      Init(new UsiAiController());
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