using MainModule.AutomationTests.Peers;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MainModule.AutomationTests
{
  [TestClass]
  public class WelcomeViewTest
  {
    private ApplicationUnderTest _application;
    private WpfWindow _window;
    private UWelcomeView _view;

    [TestInitialize]
    public void Init()
    {
      Playback.Initialize();
      var startInfo = typeof(TestStand.WelcomeView.MainWindow).Assembly.Location;
      _application = ApplicationUnderTest.Launch(startInfo, startInfo);

      _window = new WpfWindow(_application);
      _window.WindowTitles.Add("Shogi");

      _view = new UWelcomeView(_window);
    }
    [TestCleanup]
    public void Cleanup()
    {
      _application.Close();
      Playback.Cleanup();
    }
    [TestMethod]
    public void CheckExistance()
    {
      Assert.IsTrue(_view.PlayWithCompButton.WaitForControlExist());
      Assert.IsTrue(_view.PlayWithMyselfButton.WaitForControlExist());
      Assert.IsTrue(_view.ConnectButton.WaitForControlExist());
      Assert.IsTrue(_view.NameEdit.WaitForControlExist());
      Assert.IsTrue(_view.ServerCombo.WaitForControlExist());
      Assert.IsTrue(_view.LetMeBeAServerLink.WaitForControlExist());
      Assert.IsTrue(_view.DontShowThisPaheAgainCheckBox.WaitForControlExist());
      
      Assert.IsTrue(_view.IsNameValid);
      Assert.IsTrue(_view.IsServerValid);

      Assert.AreEqual("localhost", _view.ServerCombo.EditableItem);
      Assert.AreEqual("John Doe", _view.NameEdit.Text);
    }
    [TestMethod]
    public void CheckValidation()
    {
      _view.NameEdit.Text = "";
      Assert.IsFalse(_view.IsNameValid);
      Assert.IsTrue(_view.IsServerValid);
      Assert.IsFalse(_view.ConnectButton.Enabled);
      Assert.IsTrue(_view.PlayWithCompButton.Enabled);
      Assert.IsTrue(_view.PlayWithMyselfButton.Enabled);

      _view.NameEdit.Text = "Some Name";
      Assert.IsTrue(_view.IsNameValid);
      Assert.IsTrue(_view.IsServerValid);
      Assert.IsTrue(_view.ConnectButton.Enabled);
      Assert.IsTrue(_view.PlayWithCompButton.Enabled);
      Assert.IsTrue(_view.PlayWithMyselfButton.Enabled);

      _view.ServerCombo.EditableItem = "";
      Assert.IsTrue(_view.IsNameValid);
      Assert.IsFalse(_view.IsServerValid);
      Assert.IsFalse(_view.ConnectButton.Enabled);
      Assert.IsTrue(_view.PlayWithCompButton.Enabled);
      Assert.IsTrue(_view.PlayWithMyselfButton.Enabled);

      _view.NameEdit.Text = "";
      Assert.IsFalse(_view.IsNameValid);
      Assert.IsFalse(_view.IsServerValid);
      Assert.IsFalse(_view.ConnectButton.Enabled);
      Assert.IsTrue(_view.PlayWithCompButton.Enabled);
      Assert.IsTrue(_view.PlayWithMyselfButton.Enabled);

      _view.NameEdit.Text = "Some Name";
      _view.ServerCombo.EditableItem = "Some Server";
      Assert.IsTrue(_view.IsNameValid);
      Assert.IsTrue(_view.IsServerValid);
      Assert.IsTrue(_view.ConnectButton.Enabled);
      Assert.IsTrue(_view.PlayWithCompButton.Enabled);
      Assert.IsTrue(_view.PlayWithMyselfButton.Enabled);
    }
  }
}