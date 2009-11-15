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

      IPlayerGameController jackController = null;
      jackSession.InvitationReceived += i => jackController = i.Accept();

      IPlayerGameController johnController = null;
      johnSession.InvitePlay(johnSession.Users.First(), c => johnController = c);

      DispatcherUtils.WaitForAllDefferedOperations();

      c1 = jackController;
      c2 = johnController;
    }
  }
}