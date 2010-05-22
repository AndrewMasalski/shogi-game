using BoardControl.AutomationTests.Peers;
using MainModule.AutomationTests.Peers;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore.Primitives;

namespace MainModule.AutomationTests
{
  [TestClass]
  public class MainWindowTest
  {
    private ApplicationUnderTest _application;
    private WpfWindow _window;
    private UShogiBoard _board;

    [TestInitialize]
    public void Init()
    {
      Playback.Initialize();
      var startInfo = typeof(MainWindow).Assembly.Location;
      _application = ApplicationUnderTest.Launch(startInfo, startInfo);
      
      _window = new WpfWindow(_application);
      _window.WindowTitles.Add("Shogi");

      Mouse.Click(new UWelcomeView(_window).PlayWithMyselfButton);

      _board = new UShogiBoard(_window);
    }
    [TestCleanup]
    public void Cleanup()
    {
      _application.Close();
      Playback.Cleanup();
    }
    [TestMethod]
    public void CheckDragAndDrop()
    {
      _board.UsualMove("1c", "1d");
      _board.UsualMove("1g", "1f");
      Assert.IsFalse(_board.GetPiece("1d").WaitForControlNotExist());
      Assert.IsTrue(_board.GetPiece("1f").WaitForControlExist());
    }
    [TestMethod]
    public void CheckMovesHistory()
    {
      _board.UsualMove("1g", "1f");
      _board.UsualMove("1c", "1d");
      _board.UsualMove("2g", "2f");
      _board.UsualMove("2c", "2d");
      _board.UsualMove("3g", "3f");
      _board.UsualMove("3c", "3d");
      _board.UsualMove("4g", "4f");
      _board.UsualMove("4c", "4d");

      var tabPage = new WpfTabPage(_window);
      tabPage.SearchProperties[WpfTabPage.PropertyNames.Name] = "Moves";
      var listBox = new WpfList(tabPage);

      Assert.AreEqual(8, listBox.Items.Count);
      foreach (var item in listBox.Items)
      {
        item.EnsureClickable();
        Mouse.Click(item);
      }
    }
    [TestMethod]
    public void CheckNoRulesDnD()
    {
      _window.InvokeMenu("Board/Enforce rules");
      _board.UsualMove("1c", "1d");
      _window.InvokeMenu("Board/Clean");
      _board.DropMove(PT.桂, PieceColor.White, "1i");
    }
  }
}