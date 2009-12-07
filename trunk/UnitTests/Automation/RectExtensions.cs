using System.Windows;

namespace UnitTests.Automation
{
  public static class RectExtensions
  {
    public static Point Center(this Rect rect)
    {
      return rect.TopLeft + (Vector)rect.Size/2.0;
    }
  }
}