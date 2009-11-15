using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Windows;
using Yasc.ShogiCore.Utils;

namespace Yasc.Networking
{
  public partial class Server : MarshalByRefObject
  {
    #region ' Static Stuff '

    private static int _port = 1937;
    private static bool _backwardChannelIsRegistred;

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

    public static Server Start()
    {
      var provider = new BinaryServerFormatterSinkProvider
                       {
                         TypeFilterLevel = TypeFilterLevel.Full
                       };
      IDictionary properties = new Hashtable();
      properties["name"] = "some name";
      properties["port"] = Port;
      ChannelServices.RegisterChannel(new TcpChannel(properties, null, provider), false);

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
        var chnl = new TcpChannel(PortUtils.GetFreePortToListen());
        ChannelServices.RegisterChannel(chnl, false);
        _backwardChannelIsRegistred = true;
      }

      url = string.Format("tcp://{0}:{1}/{2}", url, Port, typeof(Server).FullName);
      return (Server)Activator.GetObject(typeof(Server), url);
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

    private IInvitorTicket Invite(ServerUser from, ServerUser to)
    {
      var invitorTicket = new InvitorTicket(from);
      _users[to].ReceiveInvitation(new InviteeTicket(this, invitorTicket, to));
      return invitorTicket;
    }
    private IPlayerGameController InviteAccepted(InvitorTicket invitorTicket, ServerUser invitee)
    {
      var game = new ServerGame(this, invitorTicket.Invitor, invitee);
      _games.Add(game);
      invitorTicket.InvokeInvitationAccepted(game.WhitePlayer);
      return game.BlackPlayer;
    }
  }
}