using System;

namespace Yasc.Networking
{
  public partial class Server 
  {
    private class InviteeTicket : MarshalByRefObject, IInviteeTicket
    {
      private readonly Server _server;
      private readonly InvitorTicket _invitor;
      private readonly ServerUser _invitee;

      public InviteeTicket(Server server, InvitorTicket invitor, ServerUser invitee)
      {
        _server = server;
        _invitor = invitor;
        _invitee = invitee;
      }

      public IPlayerGameController Accept()
      {
        return _server.InviteAccepted(_invitor, _invitee);
      }
    }
  }
}