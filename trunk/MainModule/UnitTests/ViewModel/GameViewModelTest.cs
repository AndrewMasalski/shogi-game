using System;
using System.Linq;
using MainModule.Gui.Game;
using MainModule.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Notations;
using Yasc.ShogiCore.Primitives;

namespace MainModule.UnitTests.ViewModel
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
      var model = GameWithEngineViewModel.Create();
      Assert.AreEqual("You", model.Ticket.Me.Name);
      Assert.AreEqual(PieceColor.White, model.Ticket.MyColor);
      Assert.IsTrue(model.IsItMyMove);
      Assert.IsFalse(model.IsItOpponentMove);
      model.Board.MakeMove(model.Board.GetMove("1g-1f", FormalNotation.Instance).First());
      Assert.IsFalse(model.IsItMyMove);
      Assert.IsTrue(model.IsItOpponentMove);
    }
    [TestMethod]
    public void TimerTest()
    {
      var model = new AutoplayViewModel
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

      model.Board.MakeMove(model.Board.GetMove("1g-1f", FormalNotation.Instance).First());

      Assert.AreEqual(TimeSpan.FromMinutes(1), model.MyTime);
      Assert.IsFalse(model.IsMyTimerLaunched, "#4");
      Assert.IsFalse(model.IsItMyMove, "#5");

      Assert.AreEqual(TimeSpan.FromMinutes(1), model.OpponentTime);
      Assert.IsTrue(model.IsOpponentTimerLaunched);
      Assert.IsTrue(model.IsItOpponentMove);

      model.Board.MakeMove(model.Board.GetMove("1c-1d", FormalNotation.Instance).First());

      Assert.AreEqual(TimeSpan.FromMinutes(1), model.MyTime);
      Assert.IsTrue(model.IsMyTimerLaunched, "#6");
      Assert.IsTrue(model.IsItMyMove, "#7");
    }

    [TestMethod]
    public void AiTimerTest()
    {
      var model = GameWithEngineViewModel.Create();
      model.MyTime = TimeSpan.FromMinutes(1);
      model.OpponentTime = TimeSpan.FromMinutes(1);

      Assert.AreEqual(TimeSpan.FromMinutes(1), model.MyTime);
      Assert.IsFalse(model.IsMyTimerLaunched, "#1");
      Assert.IsTrue(model.IsItMyMove);

      Assert.AreEqual(TimeSpan.FromMinutes(1), model.OpponentTime);
      Assert.IsFalse(model.IsOpponentTimerLaunched, "#2");
      Assert.IsFalse(model.IsItOpponentMove, "#3");
    }
  }
}