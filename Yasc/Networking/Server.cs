using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Windows;
using System.Windows.Threading;
using Yasc.ShogiCore.Utils;
using System.Linq;

namespace Yasc.Networking
{
  public partial class Server : MarshalByRefObject
  {
    #region ' Static Stuff '

    private static int _port = 1937;
    private static bool _backwardChannelIsRegistred;
    private static Dispatcher _disp;

    public static int Port
    {
      get { return _port; }
      set
      {
        if (ThisDomainIsServerHost)
        {
          throw new InvalidOperationException(
            "Can't change server port when it is started");
        }
        if (_port == value) return;
        _port = value;
      }
    }
    public static bool ThisDomainIsServerHost { get; private set; }
    public static bool ServerIsStartedOnThisComputer
    {
      get { return PortUtils.IsPortBusy(Port); }
    }

    public IServerUser[] Users
    {
      get { return _users.Keys.Cast<IServerUser>().ToArray(); }
      set { throw new NotImplementedException(); }
    }

    public static Server Start()
    {
      _disp = Dispatcher.CurrentDispatcher;
      RegisterChannel(Port);

      RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;
      var server = new Server();
      RemotingServices.Marshal(server, typeof(Server).FullName);

      ThisDomainIsServerHost = true;
      return server;
    }
    public static Server Connect(string url)
    {
      if (!_backwardChannelIsRegistred && !ThisDomainIsServerHost)
      {
        // We need a channel to get messages back from the server
        // doesn't matter what port it will have
        RegisterChannel(PortUtils.GetFreePortToListen());
        _backwardChannelIsRegistred = true;
      }

      url = string.Format("tcp://{0}:{1}/{2}", url, Port, typeof(Server).FullName);
      return (Server)Activator.GetObject(typeof(Server), url);
    }

    private static void RegisterChannel(int port)
    {
      var provider = new BinaryServerFormatterSinkProvider
      {
        TypeFilterLevel = TypeFilterLevel.Full
      };
      IDictionary properties = new Hashtable();
      properties["name"] = "backward channel";
      properties["port"] = port;
      ChannelServices.RegisterChannel(new TcpChannel(properties, null, provider), false);
    }

    #endregion

    private readonly Dictionary<ServerUser, ServerSession> _users = new Dictionary<ServerUser, ServerSession>();
    private readonly List<ServerGame> _games = new List<ServerGame>();

    public IServerSession Login(string name)
    {
      var user = new ServerUser(name);
      var session = new ServerSession(this, user);
      _users.Add(user, session);
      return session;
    }
    public void Ping()
    {
      // Do nothing
    }

    private void Invite(ServerUser from, Action<IPlayerGameController> invitationAccepted, ServerUser to)
    {
      _users[to].ReceiveInvitation(new InviteeTicket(this, from, invitationAccepted, to));
    }
    private IPlayerGameController InviteAccepted(ServerUser from, Action<IPlayerGameController> invitationAccepted, ServerUser invitee)
    {
      var game = new ServerGame(this, from, invitee);
      _games.Add(game);
      invitationAccepted(game.WhitePlayer);
      return game.BlackPlayer;
    }
  }
}