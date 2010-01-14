using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.Gui;
using Yasc.Properties;
using System.Linq;
using Yasc.Utils;

namespace UnitTests.ViewModel
{
  [TestClass]
  public class WelcomeViewModelTest
  {
    private TestLog _log;
    private WelcomeViewModel _model;

    [TestInitialize]
    public void Init()
    {
      Reset(Settings.Default);
      _model = new WelcomeViewModel();
      _log = new TestLog();
      _model.ChoiceDone += (s, e) => _log.Write("ChoiceDone");
    }

    [TestMethod]
    public void ConstructorTest()
    {
      Assert.AreEqual("localhost", _model.Address);
      Assert.AreEqual("John Doe", _model.UserName);
      Assert.IsFalse(_model.IsServerStartedOnThisComputer);
      Assert.IsFalse(_model.SaveAndSkip);
      Assert.AreEqual(0, _model.LastVisitedServers.Count);
      Assert.AreEqual(WelcomeChoice.None, _model.Mode);
    }
    [TestMethod]
    public void AutoplayTest()
    {
      _model.AutoplayCommand.Execute(null);

      Assert.AreEqual("localhost", _model.Address);
      Assert.AreEqual("John Doe", _model.UserName);
      Assert.IsFalse(_model.IsServerStartedOnThisComputer);
      Assert.IsFalse(_model.SaveAndSkip);
      CollectionAssert.AreEqual(new[] { "localhost" }, _model.LastVisitedServers);
      Assert.AreEqual(WelcomeChoice.Autoplay, _model.Mode);
      Assert.AreEqual("ChoiceDone", _log.ToString());

      Assert.AreEqual("localhost", Settings.Default.Address);
      Assert.AreEqual("John Doe", Settings.Default.UserName);
      Assert.AreEqual(WelcomeChoice.None, Settings.Default.DefaultStartMode);
      CollectionAssert.AreEqual(new[] { "localhost" }, Settings.Default.LoadLvs().ToList());
    }
    [TestMethod]
    public void BecomeServerTest()
    {
      _model.BecomeServerCommand.Execute(null);

      Assert.AreEqual("localhost", _model.Address);
      Assert.AreEqual("John Doe", _model.UserName);
      Assert.IsFalse(_model.IsServerStartedOnThisComputer);
      Assert.IsFalse(_model.SaveAndSkip);
      CollectionAssert.AreEqual(new[] { "localhost" }, _model.LastVisitedServers);
      Assert.AreEqual(WelcomeChoice.BecomeServer, _model.Mode);
      Assert.AreEqual("ChoiceDone", _log.ToString());

      Assert.AreEqual("localhost", Settings.Default.Address);
      Assert.AreEqual("John Doe", Settings.Default.UserName);
      Assert.AreEqual(WelcomeChoice.None, Settings.Default.DefaultStartMode);
      CollectionAssert.AreEqual(new[] { "localhost" }, Settings.Default.LoadLvs().ToList());
    }
    [TestMethod]
    public void ConnectTest()
    {
      _model.ConnectCommand.Execute(null);

      Assert.AreEqual("localhost", _model.Address);
      Assert.AreEqual("John Doe", _model.UserName);
      Assert.IsFalse(_model.IsServerStartedOnThisComputer);
      Assert.IsFalse(_model.SaveAndSkip);
      CollectionAssert.AreEqual(new[] { "localhost" }, _model.LastVisitedServers);
      Assert.AreEqual(WelcomeChoice.ConnectToServer, _model.Mode);
      Assert.AreEqual("ChoiceDone", _log.ToString());

      Assert.AreEqual("localhost", Settings.Default.Address);
      Assert.AreEqual("John Doe", Settings.Default.UserName);
      Assert.AreEqual(WelcomeChoice.None, Settings.Default.DefaultStartMode);
      CollectionAssert.AreEqual(new[] { "localhost" }, Settings.Default.LoadLvs().ToList());
    }

    private static void Reset(Settings settings)
    {
      settings.Address = null;
      settings.UserName = null;
      settings.DefaultStartMode = WelcomeChoice.None;
      settings.SaveLvs(new string[0]);
    }
  }
}