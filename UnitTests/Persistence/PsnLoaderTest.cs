using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.Properties;
using Yasc.Persistence;

namespace UnitTests.Persistence
{
  [TestClass]
  public class PsnLoaderTest
  {
    [TestMethod]
    public void Load()
    {
      var board = new PsnLoader().Load(Resources.Game1_psn);
      Assert.AreEqual("Habu Yoshiharu", board.Black.Name);
      Assert.AreEqual("Tanigawa Koji", board.White.Name);
      Assert.AreNotEqual(0, board.History.Count);
    }
  }
}