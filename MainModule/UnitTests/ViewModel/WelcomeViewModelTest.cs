using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.Gui;
using Yasc.Properties;
using System.Linq;
using Yasc.Utils;

namespace UnitTests.ViewModel
{
  [TestClass]
  public class WelcomeViewModelWithNoSettingsTest
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
      _model.PropertyChanged += (s, e) => _log.Write("PropertyChanged(" + e.PropertyName + ")");
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

      Check(WelcomeChoice.Autoplay);
    }
    [TestMethod]
    public void BecomeServerTest()
    {
      _model.BecomeServerCommand.Execute(null);
      Check(WelcomeChoice.BecomeServer);
    }
    [TestMethod]
    public void ConnectTest()
    {
      _model.ConnectCommand.Execute(null);
      Check(WelcomeChoice.ConnectToServer);
    }
    [TestMethod]
    public void PlayWithCompTest()
    {
      _model.PlayWithCompCommand.Execute(null);
      Check(WelcomeChoice.ArtificialIntelligence);
    }

    [TestMethod]
    public void AutoplayAndSaveTest()
    {
      _model.SaveAndSkip = true;
      _model.AutoplayCommand.Execute(null);
      Check(WelcomeChoice.Autoplay, true);
    }
    [TestMethod]
    public void BecomeServerAndSaveTest()
    {
      _model.SaveAndSkip = true;
      _model.BecomeServerCommand.Execute(null);
      Check(WelcomeChoice.BecomeServer, true);
    }
    [TestMethod]
    public void ConnectAndSaveTest()
    {
      _model.SaveAndSkip = true;
      _model.ConnectCommand.Execute(null);
      Check(WelcomeChoice.ConnectToServer, true);
    }
    [TestMethod]
    public void PlayWithCompAndSaveTest()
    {
      _model.SaveAndSkip = true;
      _model.PlayWithCompCommand.Execute(null);
      Check(WelcomeChoice.ArtificialIntelligence, true);
    }

    private static void Reset(Settings settings)
    {
      settings.Address = null;
      settings.UserName = null;
      settings.DefaultStartMode = WelcomeChoice.None;
      settings.SaveLvs(new string[0]);
    }

    private void Check(WelcomeChoice expectedMode, bool saveAndSkip = false)
    {
      Assert.AreEqual("localhost", _model.Address);
      Assert.AreEqual("John Doe", _model.UserName);
      Assert.IsFalse(_model.IsServerStartedOnThisComputer);
      Assert.AreEqual(saveAndSkip, _model.SaveAndSkip);
      CollectionAssert.AreEqual(new[] { "localhost" }, _model.LastVisitedServers);
      Assert.AreEqual(expectedMode, _model.Mode);
      if (saveAndSkip)
      {
        Assert.AreEqual("PropertyChanged(SaveAndSkip) PropertyChanged(Mode) ChoiceDone",
          _log.ToString());
      }
      else
      {
        Assert.AreEqual("PropertyChanged(Mode) ChoiceDone", _log.ToString());
      }

      Assert.AreEqual("localhost", Settings.Default.Address);
      Assert.AreEqual("John Doe", Settings.Default.UserName);
      Assert.AreEqual(
        saveAndSkip ? expectedMode : WelcomeChoice.None,
        Settings.Default.DefaultStartMode);

      CollectionAssert.AreEqual(
        new[] { "localhost" }, Settings.Default.LoadLvs().ToList());
    }
  }
  [TestClass]
  public class WelcomeViewModelWithSettingsTest
  {
    private TestLog _log;
    private WelcomeViewModel _model;

    [TestInitialize]
    public void Init()
    {
      Settings.Default.Address = "Address";
      Settings.Default.UserName = "UserName";
      Settings.Default.DefaultStartMode = WelcomeChoice.BecomeServer;
      Settings.Default.SaveLvs(new[] { "a", "b" });

      _model = new WelcomeViewModel();
      _log = new TestLog();
      _model.ChoiceDone += (s, e) => _log.Write("ChoiceDone");
      _model.PropertyChanged += (s, e) => _log.Write("PropertyChanged(" + e.PropertyName + ")");
    }

    [TestMethod]
    public void ConstructorTest()
    {
      Assert.AreEqual("Address", _model.Address);
      Assert.AreEqual("UserName", _model.UserName);
      Assert.IsFalse(_model.IsServerStartedOnThisComputer);
      Assert.IsTrue(_model.SaveAndSkip);
      CollectionAssert.AreEqual(new []{"a", "b"}, _model.LastVisitedServers);
      Assert.AreEqual(WelcomeChoice.None, _model.Mode);
    }
    [TestMethod]
    public void AutoplayTest()
    {
      _model.AutoplayCommand.Execute(null);

      Check(WelcomeChoice.Autoplay);
    }
    [TestMethod]
    public void BecomeServerTest()
    {
      _model.BecomeServerCommand.Execute(null);
      Check(WelcomeChoice.BecomeServer);
    }
    [TestMethod]
    public void ConnectTest()
    {
      _model.ConnectCommand.Execute(null);
      Check(WelcomeChoice.ConnectToServer);
    }
    [TestMethod]
    public void PlayWithCompTest()
    {
      _model.PlayWithCompCommand.Execute(null);
      Check(WelcomeChoice.ArtificialIntelligence);
    }

    [TestMethod]
    public void AutoplayAndSaveTest()
    {
      _model.SaveAndSkip = true;
      _model.AutoplayCommand.Execute(null);
      Check(WelcomeChoice.Autoplay);
    }
    [TestMethod]
    public void BecomeServerAndSaveTest()
    {
      _model.SaveAndSkip = true;
      _model.BecomeServerCommand.Execute(null);
      Check(WelcomeChoice.BecomeServer);
    }
    [TestMethod]
    public void ConnectAndSaveTest()
    {
      _model.SaveAndSkip = true;
      _model.ConnectCommand.Execute(null);
      Check(WelcomeChoice.ConnectToServer);
    }
    [TestMethod]
    public void PlayWithCompAndSaveTest()
    {
      _model.SaveAndSkip = true;
      _model.PlayWithCompCommand.Execute(null);
      Check(WelcomeChoice.ArtificialIntelligence);
    }

    private void Check(WelcomeChoice expectedMode)
    {
      // TODO: Do we really need separate "Address" setting?
      Assert.AreEqual("Address", _model.Address);
      Assert.AreEqual("UserName", _model.UserName);
      Assert.IsFalse(_model.IsServerStartedOnThisComputer);
      Assert.IsTrue(_model.SaveAndSkip);
      CollectionAssert.AreEqual(new[] { "Address", "a", "b" }, _model.LastVisitedServers);
      Assert.AreEqual(expectedMode, _model.Mode);
      Assert.AreEqual("PropertyChanged(Mode) ChoiceDone", _log.ToString());

      Assert.AreEqual("Address", Settings.Default.Address);
      Assert.AreEqual("UserName", Settings.Default.UserName);
      Assert.AreEqual(expectedMode, Settings.Default.DefaultStartMode);

      CollectionAssert.AreEqual(
        new[] { "Address", "a", "b" }, Settings.Default.LoadLvs().ToList());
    }

    [TestMethod]
    public void SetUserNameAndMode()
    {
      _model.UserName = "new";
      _model.Mode = _model.Mode;
      Assert.AreEqual("new", _model.UserName);
      Assert.AreEqual("PropertyChanged(UserName)", _log.ToString());
      _model.ConnectCommand.Execute(null);
      Assert.AreEqual("new", Settings.Default.UserName);
    }
    [TestMethod]
    public void SetNewAddressTest()
    {
      _model.Address = "new";
      Assert.AreEqual("new", _model.Address);
      CollectionAssert.AreEqual(
        new[] {"a", "b" }, _model.LastVisitedServers);
      Assert.AreEqual("PropertyChanged(Address)", _log.ToString());
      _model.ConnectCommand.Execute(null);
      Assert.AreEqual("new", Settings.Default.Address);
      CollectionAssert.AreEqual(
        new[] { "new", "a", "b" }, _model.LastVisitedServers);
      CollectionAssert.AreEqual(
        new[] { "new", "a", "b" }, Settings.Default.LoadLvs().ToList());
    }
    [TestMethod]
    public void SetExistingAddressTest()
    {
      _model.Address = "b";
      Assert.AreEqual("b", _model.Address);
      CollectionAssert.AreEqual(
        new[] { "a", "b" }, _model.LastVisitedServers);
      Assert.AreEqual("PropertyChanged(Address)", _log.ToString());
      _model.ConnectCommand.Execute(null);
      Assert.AreEqual("b", Settings.Default.Address);
      CollectionAssert.AreEqual(
        new[] { "b", "a" }, _model.LastVisitedServers);
      CollectionAssert.AreEqual(
        new[] { "b", "a" }, Settings.Default.LoadLvs().ToList());
    }

  }
}