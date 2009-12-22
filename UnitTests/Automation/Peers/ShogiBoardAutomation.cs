using System.Windows.Automation;
using System.Windows.Input;
using Yasc.Controls;
using Yasc.ShogiCore;

namespace UnitTests.Automation
{
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
    public ShogiHandAutomation TopHand
    {
      get { return new ShogiHandAutomation(Element.FindFirstByName("TopHand")); }
    }
    public ShogiHandAutomation BottomHand
    {
      get { return new ShogiHandAutomation(Element.FindFirstByName("BottomHand")); }
    }
    public ShogiHandAutomation Hand(PieceColor handColor)
    {
      return handColor == PieceColor.Black ? BottomHand : TopHand;
    }
    public void UsusalMove(Position from, Position to)
    {
      Mouse.PrimaryDevice.DragAndDrop(
        this[from].Element.Center(),
        this[to].Element.Center(),
        MouseButton.Left);
    }

    public void DropMove(PieceType pieceType, PieceColor fromHand, Position toPosition)
    {
      Mouse.PrimaryDevice.DragAndDrop(
        Hand(fromHand)[pieceType].Element.Center(),
        this[toPosition].Element.Center(),
        MouseButton.Left);
    }
  }
}