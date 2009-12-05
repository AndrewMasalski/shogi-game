using System.Windows.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        new PropertyCondition(AutomationElement.NameProperty, "歩"));
      Assert.IsNotNull(piece);
    }

    [TestCleanup]
    public void TearDown()
    {
      _app.Close();
      _windowElement = null;
    }
  }
}
