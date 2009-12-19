using System.Threading;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.Controls;
using Yasc.ShogiCore;
using System;

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
      _windowElement.Pattern<WindowPattern>().SetWindowVisualState(WindowVisualState.Maximized);
      _windowElement.InvokeByName("Play with comp.");
      var piece = _windowElement.FindFirstByName("White P");
      Assert.IsNotNull(piece);
      var pieces = _windowElement.FindAll(typeof (ShogiPiece), 40);
      Assert.AreEqual(40, pieces.Count);

      var board = new ShogiBoardAutomation(_windowElement);
      Mouse.PrimaryDevice.PressAt(board["1c"].Element.Center(), MouseButton.Left);
      Mouse.PrimaryDevice.Release(MouseButton.Left);
      board.Move("1c", "1d");
    }
    [TestMethod]
    public void CheckMovesHistory()
    {
      _windowElement.InvokeByName("Play with comp.");

      var board = new ShogiBoardAutomation(_windowElement);
      board.Move("1c", "1d");
      board.Move("1g", "1f");
      board.Move("2c", "2d");
      board.Move("2g", "2f");
      board.Move("3c", "3d");
      board.Move("3g", "3f");
      board.Move("4c", "4d");
      board.Move("4g", "4f");

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
      _windowElement.InvokeByName("Play with comp.");
      _windowElement.InvokeByName("Enforce rules");
      new ShogiBoardAutomation(_windowElement).Move("1c", "1d");

    }
    [TestCleanup]
    public void TearDown()
    {
      _app.Close();
      _windowElement = null;
    }
  }

  public abstract class ConcreteAutomation<T>
  {
    public AutomationElement Element { get; private set; }

    protected ConcreteAutomation(AutomationElement element)
    {
      if (element == null) throw new ArgumentNullException("element");
      string expectedType = typeof(T).Name;
      if (element.Current.ClassName != expectedType)
      {
        element = element.FindFirst(typeof (T));
        if (element == null)
          throw new ArgumentOutOfRangeException("element",
            "Given element is not of type " + expectedType + 
            " and doesn't contain automation tree children of that type");
      }

      Element = element;
    }
  }
  public class ShogiBoardAutomation : ConcreteAutomation<ShogiBoard>
  {
    public ShogiBoardAutomation(AutomationElement element)
      : base(element)
    {
    }

    public ShogiCellAutomation this[Position position]
    {
      get { return new ShogiCellAutomation(Element.FindFirstByName(position.ToString())); }
    }
    public void Move(Position from, Position to)
    {
      Mouse.PrimaryDevice.DragAndDrop(
        this[from].Element.Center(),
        this[to].Element.Center(),
        MouseButton.Left);
      
    }
  }
  public class ShogiCellAutomation : ConcreteAutomation<ShogiCell>
  {
    public ShogiCellAutomation(AutomationElement element)
      : base(element)
    {
    }
  }

  public static class AutomationExtensions
  {
    private const int WiatCyclesCount = 3;

    public static AutomationElement FindFirstByName(this AutomationElement element, string name)
    {
      var result = element.FindFirstByNameNoWait(name);
      for (int i = 0; i < WiatCyclesCount && result == null; i++)
      {
        Thread.Sleep(0);
        result = element.FindFirstByNameNoWait(name);
      }
      return result;
    }
    public static AutomationElement FindFirstByName(this AutomationElement element, Type type, string name)
    {
      var result = element.FindFirstByNameNoWait(type, name);
      for (int i = 0; i < WiatCyclesCount && result == null; i++)
      {
        Thread.Sleep(0);
        result = element.FindFirstByNameNoWait(type, name);
      }
      return result;
    }
    public static AutomationElement FindFirstByNameNoWait(this AutomationElement element, string name)
    {
      return element.FindFirst(TreeScope.Descendants,
         new PropertyCondition(AutomationElement.NameProperty, name));
    }
    public static AutomationElement FindFirstByNameNoWait(this AutomationElement element, Type type, string name)
    {
      return element.FindFirst(TreeScope.Descendants, new AndCondition(
        new PropertyCondition(AutomationElement.ClassNameProperty, type.Name),
        new PropertyCondition(AutomationElement.NameProperty, name)));
    }

    public static AutomationElement FindFirst(this AutomationElement element, Type type)
    {
      var result = element.FindFirstNoWait(type);
      for (int i = 0; i < WiatCyclesCount && result == null; i++)
      {
        Thread.Sleep(0);
        result = element.FindFirstNoWait(type);
      }
      return result;
    }
    public static AutomationElement FindFirstNoWait(this AutomationElement element, Type type)
    {
      return element.FindFirst(TreeScope.Descendants,
        new PropertyCondition(AutomationElement.ClassNameProperty, type.Name));
    }

    public static AutomationElementCollection FindAll(this AutomationElement element, Type type, int expectedCount)
    {
      var result = element.FindAllNoWait(type);
      for (int i = 0; i < WiatCyclesCount && result.Count < expectedCount; i++)
      {
        Thread.Sleep(0);
        result = element.FindAllNoWait(type);
      }
      return result;
    }
    public static AutomationElementCollection FindAllNoWait(this AutomationElement element, Type type)
    {
      return element.FindAll(TreeScope.Descendants,
        new PropertyCondition(AutomationElement.ClassNameProperty, type.Name));
    }

    public static void InvokeByName(this AutomationElement element, string name)
    {
      ((InvokePattern)element.FindFirstByName(name).
        GetCurrentPattern(InvokePattern.Pattern)).Invoke();
    }
    public static T Pattern<T>(this AutomationElement element)
      where T : BasePattern
    {
      var pattern = (AutomationPattern)typeof(T).GetField("Pattern").GetValue(null);
      return (T)element.GetCurrentPattern(pattern);
    }
  }
}
