using MainModule.Gui;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.Networking;

namespace MainModule.UnitTests.ViewModel
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
}
