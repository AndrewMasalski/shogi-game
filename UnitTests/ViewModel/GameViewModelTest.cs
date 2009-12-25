using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.Gui;
using Yasc.ShogiCore;

namespace UnitTests.ViewModel
{
  [TestClass]
  public class GameViewModelTest
  {
    [TestMethod]
    public void ClearBoard()
    {
      var model = new GameViewModel(WelcomeChoice.Autoplay);
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
      var model = new GameViewModel(WelcomeChoice.ArtificialIntelligence);
      Assert.AreEqual("You", model.Ticket.Me.Name);
      Assert.AreEqual(PieceColor.White, model.Ticket.MyColor);
      Assert.IsTrue(model.IsItMyMove);
      Assert.IsFalse(model.IsItOpponentMove);
      model.Board.MakeMove(model.Board.GetMove("1c-1d"));
      Assert.IsFalse(model.IsItMyMove);
      Assert.IsTrue(model.IsItOpponentMove);
    }
  }
}