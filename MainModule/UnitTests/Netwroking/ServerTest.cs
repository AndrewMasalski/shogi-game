﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.AI;
using Yasc.Gui;
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
      var server = new ShogiServer();
      var johnSession = server.LogOn("John");
      var jackSession = server.LogOn("Jack");

      CollectionAssert.AreEqual(new[] { "Jack" },
        (from u in johnSession.Users select u.Name).ToList());

      CollectionAssert.AreEqual(new[] { "John" },
        (from u in jackSession.Users select u.Name).ToList());
    }

    [TestMethod]
    public void TestInvitation()
    {
      var server = new ShogiServer();

      var johnSession = server.LogOn("John");
      var jackSession = server.LogOn("Jack");

      IPlayerGameController jackController = null;
      jackSession.InvitationReceived += i => jackController = i.Accept();

      IPlayerGameController johnController = null;
      johnSession.InvitePlay(johnSession.Users.First(), c => johnController = c);

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

    public void TestInvColor()
    {
      var server = new ShogiServer();

      var johnSession = server.LogOn("John");
      var jackSession = server.LogOn("Jack");

      IPlayerGameController jackController = null;
      jackSession.InvitationReceived += i => jackController = i.Accept();

      IPlayerGameController johnController = null;
      johnSession.InvitePlay(johnSession.Users.First(), c => johnController = c);

      // John invited jack
      Assert.AreNotEqual(jackController.MyColor, johnController.MyColor);
      var game = jackController.Game;
      Assert.AreEqual("John", game.Invitor.Name);
      Assert.AreEqual("Jack", game.Invitee.Name);
      Assert.AreEqual(jackController.MyColor, game.InviteeColor);
    }
  }
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
  [TestClass]
  public class AiControllerTest
  {
    [TestMethod]
    public void Test()
    {
      var ticket = new GameTicket(new CleverAiController(), m => DateTime.Now);
      Assert.AreEqual("You", ticket.Me.Name);
    }
  }
}