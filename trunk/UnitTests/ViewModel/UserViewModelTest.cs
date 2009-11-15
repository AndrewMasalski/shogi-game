using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.Gui;
using Yasc.Networking;

namespace UnitTests.ViewModel
{
  [TestClass]
  public class UserViewModelTest
  {
    [TestMethod]
    public void TestEvents()
    {
      var server = new Server();
      var session = server.Login("john");
      var model = new UserViewModel(session, server.Users[0]);
      int[] counter = {0};
      model.Invited += (s,e) => counter[0]++;
      Assert.AreEqual(1, counter[0]);

      var jack = server.Login("jack");
      jack.InvitePlay(jack.Users[0], controller => counter[0]++);
      Assert.AreEqual(2, counter[0]);
    }
  }
}
