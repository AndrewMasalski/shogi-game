using System.Windows;

namespace Yasc.Utils.Automation
{
  public static class RectExtensions
  {
    public static Point Center(this Rect rect)
    {
      return rect.TopLeft + (System.Windows.Vector)rect.Size/2.0;
    }
  }
}