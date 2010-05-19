using BoardControl.AutomationTests.Peers;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStand.ShogiBoard;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Primitives;

namespace BoardControl.AutomationTests
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
      _board.UsusalMove("1c", "1d");
      _board.UsusalMove("1g", "1f");
      Assert.IsFalse(_board["1d"].Piece.WaitForControlNotExist());
      Assert.IsTrue(_board["1d"].Piece.WaitForControlExist());
    }
    [TestMethod]
    public void CheckMovesHistory()
    {
      _board.UsusalMove("1g", "1f");
      _board.UsusalMove("1c", "1d");
      _board.UsusalMove("2g", "2f");
      _board.UsusalMove("2c", "2d");
      _board.UsusalMove("3g", "3f");
      _board.UsusalMove("3c", "3d");
      _board.UsusalMove("4g", "4f");
      _board.UsusalMove("4c", "4d");

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
      _board.UsusalMove("1c", "1d");
      _window.InvokeMenu("Board/Clean");
      _board.DropMove("桂", PieceColor.White, "1i");
    }
  }
}