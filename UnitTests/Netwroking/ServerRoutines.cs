using System.Linq;
using Yasc.Networking;

namespace UnitTests.Netwroking
{
  public static class ServerRoutines
  {
    public static void CrateGame(out IPlayerGameController c1, out IPlayerGameController c2)
    {
      var server = new Server();
      
      var johnSession = server.Login("John");
      var jackSession = server.Login("Jack");

      IInviteeTicket inviteeTicket = null;
      jackSession.InvitationReceived += i => inviteeTicket = i;

      var invitorTicket = johnSession.InvitePlay(johnSession.Users.First());

      IPlayerGameController johnController = null;
      invitorTicket.InvitationAccepted += c => johnController = c;

      var jackController = inviteeTicket.Accept();

      c1 = jackController;
      c2 = johnController;
    }
  }
}