using System.Windows.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.Controls;

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
    }

    [TestCleanup]
    public void TearDown()
    {
      _app.Close();
      _windowElement = null;
    }
  }
}
