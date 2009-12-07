using System.Windows;
using System.Windows.Input;

namespace UnitTests.Automation
{
  public static class MouseDeviceExtensions
  {
    public static void PressAt(this MouseDevice device, Point point, MouseButton button)
    {
      SendInputInterop.MoveMouseCursor(point);
      SendInputInterop.ToggleMouseButton(1, button, ToggleMode.Press);
    }
    public static void Release(this MouseDevice device, MouseButton button)
    {
      SendInputInterop.ToggleMouseButton(1, button, ToggleMode.Release);
    }
    public static void ReleaseAt(this MouseDevice device, Point point, MouseButton button)
    {
      SendInputInterop.MoveMouseCursor(point);
      SendInputInterop.ToggleMouseButton(1, button, ToggleMode.Release);
    }
    public static void Click(this MouseDevice device, Point point, MouseButton button)
    {
      PressAt(device, point, button);
      Release(device, button);
    }
    public static void DoubleClick(this MouseDevice device, Point point, MouseButton button)
    {
      Click(device, point, button);
      Click(device, point, button);
    }
    public static void DragAndDrop(this MouseDevice device, Point source, Point dest, MouseButton button)
    {
      PressAt(device, source, button);
      ReleaseAt(device, dest, button);
    }
  }
}