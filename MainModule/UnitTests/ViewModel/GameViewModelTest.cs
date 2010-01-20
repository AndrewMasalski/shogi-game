using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.Gui.Game;
using Yasc.Properties;
using Yasc.ShogiCore;

namespace UnitTests.ViewModel
{
  [TestClass]
  public class GameViewModelTest
  {
    [TestInitialize]
    public void Init()
    {
      Settings.Default.CurrentEngine = @"Spear\SpearShogidokoro.exe";
    }

    [TestMethod]
    public void ClearBoard()
    {
      var model = new AutoplayViewModel();
      model.CleanBoardCommand.Execute(null);
      Assert.AreEqual(0, (from p in Position.OnBoard
                          where model.Board[p] != null 
                          select model.Board[p]).Count());
      Assert.AreEqual(40, model.Board.White.Hand.Concat(
        model.Board.Black.Hand).Count());
    }

    [TestMethod]
    public void ParticipantsTest()
    {
      var model = new GameWithEngineViewModel();
      Assert.AreEqual("You", model.Ticket.Me.Name);
      Assert.AreEqual(PieceColor.White, model.Ticket.MyColor);
      Assert.IsTrue(model.IsItMyMove);
      Assert.IsFalse(model.IsItOpponentMove);
      model.Board.MakeMove(model.Board.GetMove("1g-1f"));
      Assert.IsFalse(model.IsItMyMove);
      Assert.IsTrue(model.IsItOpponentMove);
    }
    [TestMethod]
    public void TimerTest()
    {
      var model = new AutoplayViewModel()
                    {
                      MyTime = TimeSpan.FromMinutes(1),
                      OpponentTime = TimeSpan.FromMinutes(1)
                    };

      Assert.AreEqual(TimeSpan.FromMinutes(1), model.MyTime);
      Assert.IsFalse(model.IsMyTimerLaunched, "#1");
      Assert.IsTrue(model.IsItMyMove);

      Assert.AreEqual(TimeSpan.FromMinutes(1), model.OpponentTime);
      Assert.IsFalse(model.IsOpponentTimerLaunched, "#2");
      Assert.IsFalse(model.IsItOpponentMove, "#3");

      model.Board.MakeMove(model.Board.GetMove("1g-1f"));

      Assert.AreEqual(TimeSpan.FromMinutes(1), model.MyTime);
      Assert.IsFalse(model.IsMyTimerLaunched, "#4");
      Assert.IsFalse(model.IsItMyMove, "#5");

      Assert.AreEqual(TimeSpan.FromMinutes(1), model.OpponentTime);
      Assert.IsTrue(model.IsOpponentTimerLaunched);
      Assert.IsTrue(model.IsItOpponentMove);

      model.Board.MakeMove(model.Board.GetMove("1c-1d"));

      Assert.AreEqual(TimeSpan.FromMinutes(1), model.MyTime);
      Assert.IsTrue(model.IsMyTimerLaunched, "#6");
      Assert.IsTrue(model.IsItMyMove, "#7");
    }

    [TestMethod]
    public void AiTimerTest()
    {
      var model = new GameWithEngineViewModel()
      {
        MyTime = TimeSpan.FromMinutes(1),
        OpponentTime = TimeSpan.FromMinutes(1)
      };

      Assert.AreEqual(TimeSpan.FromMinutes(1), model.MyTime);
      Assert.IsFalse(model.IsMyTimerLaunched, "#1");
      Assert.IsTrue(model.IsItMyMove);

      Assert.AreEqual(TimeSpan.FromMinutes(1), model.OpponentTime);
      Assert.IsFalse(model.IsOpponentTimerLaunched, "#2");
      Assert.IsFalse(model.IsItOpponentMove, "#3");
    }
  }
}