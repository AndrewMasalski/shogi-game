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
    public void CheckDragAndDrop()
    {
      var pattern = (WindowPattern)_windowElement.
        GetCurrentPattern(WindowPattern.Pattern);
      pattern.SetWindowVisualState(WindowVisualState.Maximized);
      InvokeByName("Play with comp.");
      var piece = _windowElement.FindFirst(TreeScope.Descendants,
        new PropertyCondition(AutomationElement.NameProperty, "White P"));
      Assert.IsNotNull(piece);
      var pieces = _windowElement.FindAll(TreeScope.Descendants,
        new PropertyCondition(AutomationElement.ClassNameProperty, typeof(ShogiPiece).Name));
      Assert.AreEqual(40, pieces.Count);

      Mouse.PrimaryDevice.PressAt(FindCell("1c").Center(), MouseButton.Left);
      Mouse.PrimaryDevice.Release(MouseButton.Left);
      Mouse.PrimaryDevice.DragAndDrop(FindCell("1c").Center(), FindCell("1d").Center(), MouseButton.Left);
    }
    [TestMethod]
    public void CheckMovesHistory()
    {
      InvokeByName("Play with comp.");
      Assert.Fail("Make move and then try rool history back");
    }

    private void InvokeByName(string name)
    {
      var element = _windowElement.FindFirst(TreeScope.Descendants,
             new PropertyCondition(AutomationElement.NameProperty, name));
      var button = (InvokePattern)element.
        GetCurrentPattern(InvokePattern.Pattern);
      button.Invoke();
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
