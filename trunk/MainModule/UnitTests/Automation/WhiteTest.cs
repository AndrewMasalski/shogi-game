using System.Threading;
using System.Windows.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using White.Core;
using White.Core.Configuration;
using White.Core.UIItems;
using White.Core.UIItems.Actions;
using White.Core.UIItems.Custom;
using White.Core.UIItems.Finders;
using Yasc.ShogiCore;

namespace MainModule.UnitTests.Automation
{
  [TestClass]
  public class WhiteTest
  {

    [TestMethod]
    public void Test()
    {
      CoreAppXmlConfiguration.Instance.BusyTimeout = 15000;
      var application = Application.Launch(typeof(MainWindow).Assembly.Location);
      var window = application.GetWindow("Shogi");
      var button = window.Get<Button>(SearchCriteria.ByText("Play with myself"));
      button.Click();
      var board = window.Get<UShogiBoard>();
      Assert.IsFalse(board.Move("1c", "1d"));
      Assert.IsTrue(board.Move("1g", "1f"));
      window.Close();
    }
    [ControlTypeMapping(CustomUIItemType.Custom)]
    public class UShogiBoard : CustomUIItem
    {
      protected UShogiBoard(AutomationElement automationElement, ActionListener actionListener)
        : base(automationElement, actionListener)
      {
      }

      protected UShogiBoard()
      {
      }

      public UShogiCell this[Position p]
      {
        get { return Container.Get<UShogiCell>(SearchCriteria.ByText(p.ToString())); }
      }

      public bool Move(Position from, Position to)
      {
        var cell = this[from];
        var piece = cell.Piece;
        Container.Mouse.DragAndDrop(piece, this[to]);
        Thread.Sleep(1000);
        return this[to].Piece != null;
      }
    }

  }

  [ControlTypeMapping(CustomUIItemType.Custom)]
  public class UShogiCell : CustomUIItem
  {
    protected UShogiCell(AutomationElement automationElement, ActionListener actionListener)
      : base(automationElement, actionListener)
    {
    }

    protected UShogiCell()
    {
    }

    public UShogiPiece Piece
    {
      get { return Container.Get<UShogiPiece>(SearchCriteria.All); }
    }
  }

  [ControlTypeMapping(CustomUIItemType.Custom)]
  public class UShogiPiece : CustomUIItem
  {
    protected UShogiPiece(AutomationElement automationElement, ActionListener actionListener)
      : base(automationElement, actionListener)
    {
    }

    protected UShogiPiece()
    {
    }
  }
}