using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using MainModule.UnitTests.Automation.Peers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc;
using Yasc.BoardControl.Controls;
using Yasc.ShogiCore;
using Yasc.Utils.Automation;

namespace MainModule.UnitTests.Automation
{
  [TestClass]
  public class MainWindowTest
  {
    private ApplicationHost _app;
    private AutomationElement _windowElement;

    [TestInitialize]
    public void SetUp()
    {
      Assert.Fail();
      _app = new ApplicationHost();
      _windowElement = _app.Open(typeof(MainWindow).Assembly.Location);
    }

    [TestMethod]
    public void CheckDragAndDrop()
    {
      _windowElement.Pattern<WindowPattern>().SetWindowVisualState(WindowVisualState.Maximized);
      _windowElement.InvokeByName("Play with myself");
      var piece = _windowElement.FindFirstByName("White P");
      Assert.IsNotNull(piece);
      var core = new ShogiBoardCoreAutomation(_windowElement);
      var pieces = core.Element.FindAll(typeof(ShogiPiece), 40);
      Assert.AreEqual(40, pieces.Count);

      var board = new ShogiBoardAutomation(_windowElement);
      Mouse.PrimaryDevice.PressAt(board["1c"].Element.Center(), MouseButton.Left);
      Mouse.PrimaryDevice.Release(MouseButton.Left);
      board.UsusalMove("1c", "1d");
    }
    [TestMethod]
    public void CheckMovesHistory()
    {
      _windowElement.InvokeByName("Play with myself");

      var board = new ShogiBoardAutomation(_windowElement);
      board.UsusalMove("1c", "1d");
      board.UsusalMove("1g", "1f");
      board.UsusalMove("2c", "2d");
      board.UsusalMove("2g", "2f");
      board.UsusalMove("3c", "3d");
      board.UsusalMove("3g", "3f");
      board.UsusalMove("4c", "4d");
      board.UsusalMove("4g", "4f");

      var moveListBoxItems = _windowElement.
        FindFirstByName(typeof(TabItem), "Moves").
        FindFirst(typeof(ListBox)).
        FindAll(typeof(ListBoxItem), 8);

      foreach (AutomationElement item in moveListBoxItems)
        item.Pattern<SelectionItemPattern>().Select();
    }

    [TestMethod]
    public void CheckNoRulesDnD()
    {
      _windowElement.InvokeByName("Play with myself");
      _windowElement.InvokeMenu("Board/Enforce rules");
      var board = new ShogiBoardAutomation(_windowElement);
      board.UsusalMove("1c", "1d");
      _windowElement.InvokeMenu("Board/Clean");
      board.DropMove(PieceType.桂, PieceColor.White, "1i");
    } 
    [TestCleanup]
    public void TearDown()
    {
      _app.Close();
      _windowElement = null;
    }
  }
}
