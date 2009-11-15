using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.Networking;
using System.Linq;

namespace UnitTests.Netwroking
{
  [TestClass]
  public class ServerTest
  {
    [TestMethod]
    public void TestUsersList()
    {
      var server = new Server();
      var johnSession = server.Login("John");
      var jackSession = server.Login("Jack");

      CollectionAssert.AreEqual(new[] { "Jack" },
        (from u in johnSession.Users select u.Name).ToList());

      CollectionAssert.AreEqual(new[] { "John" },
        (from u in jackSession.Users select u.Name).ToList());
    }

    [TestMethod]
    public void TestInvitation()
    {
      var server = new Server();

      var johnSession = server.Login("John");
      var jackSession = server.Login("Jack");

      IPlayerGameController jackController = null;
      jackSession.InvitationReceived += i => jackController = i.Accept();

      IPlayerGameController johnController = null;
      johnSession.InvitePlay(johnSession.Users.First(), c => johnController = c);

      DispatcherUtils.WaitForAllDefferedOperations();
      Assert.AreSame(jackController.Game, johnController.Game);
    }

    public void TestMoves()
    {
      IPlayerGameController c1;
      IPlayerGameController c2;
      ServerRoutines.CrateGame(out c1, out c2);

      var log1 = new TestLog();
      c1.OpponentMadeMove = m => { log1.Write(m.Move); return DateTime.Now; }; 

      var log2 = new TestLog();
      c2.OpponentMadeMove = m => { log2.Write(m.Move); return DateTime.Now; }; 

      c1.Move(new MoveMsg("m1"));
      c2.Move(new MoveMsg("m2"));
      c1.Move(new MoveMsg("m3"));
      c2.Move(new MoveMsg("m4"));

      Assert.AreEqual(log1.ToString(), "m2 m4");
      Assert.AreEqual(log1.ToString(), "m1 m3");
    }
  }
}