using System;
using MvvmFoundation.Wpf;
using Yasc.Networking;

namespace Yasc.Gui
{
  public class ServerViewModel : ObservableObject
  {
    public Server Server { get; private set; }

    public IPlayerGameController GameTicket { get; set; }

    public ServerViewModel(Server server)
    {
      Server = server;
    }

    public ServerViewModel()
    {
      Server = Server.Start();
    }

    public event EventHandler Disconnected;
    public event EventHandler Game;
    public event EventHandler GameNegotiation;
  }
}
