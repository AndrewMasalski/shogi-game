using System;

namespace Yasc.Networking
{
  public interface IServerSession
  {
    ShogiServer Server { get; }

    IServerUser[] Users { get; }
    IServerGame[] Games { get; }
    
    event Action<IInviteeTicket> InvitationReceived;
    void InvitePlay(IServerUser user, Action<IPlayerGameController> invitationAccepted);
  }
}