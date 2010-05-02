using System;
using System.Linq;
using MainModule.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.Networking;
using Yasc.Networking.Interfaces;

namespace MainModule.UnitTests
{
  [TestClass]
  public class GameTicketTest
  {
    [TestMethod]
    public void Test()
    {
      var server = new ShogiServer();

      var johnSession = server.LogOn("John");
      var jackSession = server.LogOn("Jack");

      IPlayerGameController jackController = null;
      jackSession.InvitationReceived += i => jackController = i.Accept();

      IPlayerGameController johnController = null;
      johnSession.InvitePlay(johnSession.Users.First(), c => johnController = c);

      var jackTicket = new GameTicket(jackController, m => DateTime.Now);
      var johnTicket = new GameTicket(johnController, m => DateTime.Now);

      Assert.AreEqual(jackTicket.Me, johnTicket.Opponent);
      Assert.AreEqual(johnTicket.Me, jackTicket.Opponent);

      Assert.AreEqual("Jack", jackTicket.Me.Name);
      Assert.AreEqual("John", johnTicket.Me.Name);
    }
  }
}