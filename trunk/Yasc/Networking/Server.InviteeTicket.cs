using System;

namespace Yasc.Networking
{
  public partial class Server 
  {
    private class InviteeTicket : MarshalByRefObject, IInviteeTicket
    {
      private readonly Server _server;
      private readonly ServerUser _invitor;
      private readonly Action<IPlayerGameController> _invitationAccepted;
      private readonly ServerUser _invitee;

      public InviteeTicket(Server server, ServerUser invitor, Action<IPlayerGameController> invitationAccepted, ServerUser invitee)
      {
        _server = server;
        _invitor = invitor;
        _invitationAccepted = invitationAccepted;
        _invitee = invitee;
      }

      public IPlayerGameController Accept()
      {
        return _server.InviteAccepted(_invitor, _invitationAccepted, _invitee);
      }
    }
  }
}