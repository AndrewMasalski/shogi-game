using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.Netwroking;
using Yasc.Gui;
using Yasc.Networking;

namespace UnitTests.ViewModel
{
  [TestClass, NUnit.Framework.TestFixture]
  public class UserViewModelTest
  {
    [TestMethod, NUnit.Framework.Test]
    public void TestEvents()
    {
      var server = new Server();
      var john = server.Login("john");
      var jack = server.Login("jack");
      var model = new ServerViewModel("", john);
      model.Users[0].Invite();
      
      int[] counter = { 0 };

      model.Game += (s, e) => counter[0]++;
      jack.InvitePlay(jack.Users[0], controller => counter[0]++);

      Assert.AreEqual(2, counter[0]);
    }
  }
}
