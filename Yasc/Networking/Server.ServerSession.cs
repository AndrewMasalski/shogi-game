using System;
using System.Collections.Generic;
using System.Linq;

namespace Yasc.Networking
{
  public partial class Server
  {
    private class ServerSession : MarshalByRefObject, IServerSession
    {
      private readonly ServerUser _user;

      public Server Server { get; private set; }

      public ServerSession(Server server, ServerUser user)
      {
        Server = server;
        _user = user;
      }

      public IEnumerable<IServerUser> Users
      {
        get
        {
          return from u in Server._users.Keys
                 where u != _user
                 select (IServerUser)u;
        }
      }

      public IInvitorTicket InvitePlay(IServerUser user)
      {
        return Server.Invite(_user, (ServerUser)user);
      }
      public event Action<IInviteeTicket> InvitationReceived;

      public void ReceiveInvitation(InviteeTicket ticket)
      {
        var handler = InvitationReceived;
        if (handler != null) handler(ticket);
      }
    }
  }
}