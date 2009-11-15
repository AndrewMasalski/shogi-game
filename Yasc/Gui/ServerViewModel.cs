using System;
using MvvmFoundation.Wpf;
using Yasc.Networking;

namespace Yasc.Gui
{
  public class ServerViewModel : ObservableObject
  {
    public IServerSession Session { get; internal set; }
    public Server Server { get; private set; }

    public IPlayerGameController GameTicket { get; set; }

    public ServerViewModel(IServerSession session)
    {
      Server = session.Server;
      Session = session;
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
