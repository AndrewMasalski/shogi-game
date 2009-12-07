using System.Threading;
using System.Windows.Automation;
using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.Controls;
using Yasc.ShogiCore;

namespace UnitTests.Automation
{
  [TestClass]
  public class MainWindowTest
  {
    private ApplicationHost _app;
    private AutomationElement _windowElement;

    [TestInitialize]
    public void SetUp()
    {
      _app = new ApplicationHost();
      _windowElement = _app.Open();
    }

    [TestMethod]
    public void Check()
    {
      var pattern = (WindowPattern)_windowElement.
        GetCurrentPattern(WindowPattern.Pattern);
      pattern.SetWindowVisualState(WindowVisualState.Maximized);
      var element = _windowElement.FindFirst(TreeScope.Descendants,
        new PropertyCondition(AutomationElement.NameProperty, "Play with comp."));
      var button = (InvokePattern)element.GetCurrentPattern(InvokePattern.Pattern);
      button.Invoke();
      var piece = _windowElement.FindFirst(TreeScope.Descendants,
        new PropertyCondition(AutomationElement.NameProperty, "White P"));
      Assert.IsNotNull(piece);
      var pieces = _windowElement.FindAll(TreeScope.Descendants,
        new PropertyCondition(AutomationElement.ClassNameProperty, typeof(ShogiPiece).Name));
      Assert.AreEqual(40, pieces.Count);

      Mouse.PrimaryDevice.PressAt(FindCell("1c").Center(), MouseButton.Left);
      Thread.Sleep(3000);
      Mouse.PrimaryDevice.Release(MouseButton.Left);
      Thread.Sleep(100);
      Mouse.PrimaryDevice.DragAndDrop(FindCell("1c").Center(), FindCell("1d").Center(), MouseButton.Left);
      Thread.Sleep(1000);
    }
    private AutomationElement FindCell(Position position)
    {
      return _windowElement.FindFirst(TreeScope.Descendants,
        new PropertyCondition(AutomationElement.NameProperty, position.ToString()));
    }

    [TestCleanup]
    public void TearDown()
    {
      _app.Close();
      _windowElement = null;
    }
  }
}
