using System;
using System.Linq;
using System.Windows.Threading;
using Yasc.Networking.Interfaces;

namespace Yasc.Networking
{
  public partial class ShogiServer
  {
    private class ServerSession : MarshalByRefObject, IServerSession
    {
      private readonly ServerUser _user;

      public ShogiServer Server { get; private set; }

      public ServerSession(ShogiServer server, ServerUser user)
      {
        Server = server;
        _user = user;
      }

      public IServerUser[] Users
      {
        get
        {
          return (from u in Server._users.Keys
                 where u != _user
                 select (IServerUser)u).ToArray();
        }
      }

      public IServerGame[] Games
      {
        get { return (from g in Server._games select (IServerGame) g).ToArray(); }
      }

      public void InvitePlay(IServerUser user, Action<IPlayerGameController> invitationAccepted)
      {
        Server.Invite(_user, invitationAccepted, (ServerUser)user);
      }
      public event Action<IInviteeTicket> InvitationReceived;

      public void ReceiveInvitation(InviteeTicket ticket)
      {
        var handler = InvitationReceived;
        if (handler != null)
        {
          if (_disp == null)
            handler(ticket);
          else
            _disp.BeginInvoke(DispatcherPriority.Input, new Action(() => handler(ticket)));
        }
      }
    }
  }
}