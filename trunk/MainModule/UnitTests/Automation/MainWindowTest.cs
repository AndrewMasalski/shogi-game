using System.Threading;
using System.Windows.Automation;
using MainModule.UnitTests.Automation.Peers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using White.Core;
using White.Core.Configuration;
using White.Core.UIItems;
using White.Core.UIItems.Finders;
using White.Core.UIItems.ListBoxItems;
using White.Core.UIItems.MenuItems;
using White.Core.UIItems.TabItems;
using White.Core.UIItems.WindowItems;
using White.Core.UIItems.WPFUIItems;
using Yasc.ShogiCore;
using System.Linq;

namespace MainModule.UnitTests.Automation
{
  [TestClass]
  public class MainWindowTest
  {
    private Application _application;
    private Window _window;
    private UShogiBoard _board;

    [TestInitialize]
    public void Init()
    {
      CoreAppXmlConfiguration.Instance.BusyTimeout = 15000;
      _application = Application.Launch(typeof(MainWindow).Assembly.Location);
      _window = _application.GetWindow("Shogi");
      var button = _window.Get<Button>(SearchCriteria.ByText("Play with myself"));
      button.Click();
      _board = _window.Get<UShogiBoard>();
    }
    [TestCleanup]
    public void Cleanup()
    {
      _window.Close();
    }
    [TestMethod]
    public void CheckDragAndDrop()
    {
      _board.UsusalMove("1c", "1d");
      _board.UsusalMove("1g", "1f");
      Thread.Sleep(1000);
      Assert.IsNull(_board["1d"].Piece);
      Assert.IsNotNull(_board["1f"].Piece);
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

      Thread.Sleep(1000);

//      var history = _window.Get<UMovesHistory>(SearchCriteria.ByText("Moves"));
      var tabPage = _window.Get<TabPage>(SearchCriteria.ByText("Moves"));
      var listBox = tabPage.Get<ListBox>(SearchCriteria.All);
//      listBox.Items
//      var moveListBoxItems = listBox.GetMultiple(
//        SearchCriteria.Indexed(0)
//        SearchCriteria.All
//        SearchCriteria.ByControlType(ControlType.ListItem)
//        );

//      var count = listBox.Items.Count;
      Assert.AreEqual(8, listBox.Items.Count);
      foreach (var item in  listBox.Items)
        item.Select();
    }

    [TestMethod]
    public void CheckNoRulesDnD()
    {
      _window.Get<Menu>(SearchCriteria.ByText("Board"))
        .SubMenu("Enforce rules").Click();
      _board.UsusalMove("1c", "1d");
      _window.Get<Menu>(SearchCriteria.ByText("Board"))
        .SubMenu("Clean").Click();
      //      _window.InvokeMenu("Board/Clean");
      _board.DropMove("桂", PieceColor.White, "1i");
    } 
  }
}