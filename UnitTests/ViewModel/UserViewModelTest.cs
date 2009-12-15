using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.Netwroking;
using Yasc.Gui;
using Yasc.Networking;
using Yasc.ShogiCore;

namespace UnitTests.ViewModel
{
  [TestClass]
  public class UserViewModelTest
  {
    [TestMethod]
    public void TestEvents()
    {
      var server = new ShogiServer();
      var john = server.LogOn("john");
      var jack = server.LogOn("jack");
      var model = new ServerViewModel("", john);
      model.Users[0].Invite();
      
      int[] counter = { 0 };

      model.Game += (s, e) => counter[0]++;
      jack.InvitePlay(jack.Users[0], controller => counter[0]++);

      Assert.AreEqual(2, counter[0]);
    }
  }
  [TestClass]
  public class GameViewModelClass
  {
    [TestMethod]
    public void ClearBoard()
    {
      var model = new GameViewModel(WelcomeChoice.Autoplay);
      model.CleanBoardCommand.Execute(null);
      Assert.AreEqual(0, (from p in Position.OnBoard
                          where model.Board[p] != null 
                          select model.Board[p]).Count());
      Assert.AreEqual(40,
        model.Board.White.Hand.Concat(
        model.Board.Black.Hand).Count());
    }
  }
}
