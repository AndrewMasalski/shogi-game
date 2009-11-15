using System;

namespace Yasc.Networking
{
  public partial class Server 
  {
    private class InvitorTicket : MarshalByRefObject, IInvitorTicket
    {
      public event Action<IPlayerGameController> InvitationAccepted;

      public ServerUser Invitor { get; private set; }

      public InvitorTicket(ServerUser invitor)
      {
        Invitor = invitor;
      }

      public void InvokeInvitationAccepted(IPlayerGameController obj)
      {
        var accepted = InvitationAccepted;
        if (accepted != null) accepted(obj);
      }
    }
  }
}