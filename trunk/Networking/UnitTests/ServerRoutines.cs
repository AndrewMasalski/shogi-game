using System.Linq;
using Yasc.Networking;

namespace UnitTests
{
  public static class ServerRoutines
  {
    public static void CrateGame(out IPlayerGameController c1, out IPlayerGameController c2)
    {
      var server = new ShogiServer();
      
      var johnSession = server.LogOn("John");
      var jackSession = server.LogOn("Jack");

      IPlayerGameController jackController = null;
      jackSession.InvitationReceived += i => jackController = i.Accept();

      IPlayerGameController johnController = null;
      johnSession.InvitePlay(johnSession.Users.First(), c => johnController = c);

      c1 = jackController;
      c2 = johnController;
    }
  }
}