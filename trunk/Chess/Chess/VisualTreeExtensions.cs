using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Chess
{
  public static class VisualTreeExtensions
  {
    public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject element)
      where T : DependencyObject
    {
      if (element == null) yield break;
      if (element is T)
        yield return (T)element;

      var count = VisualTreeHelper.GetChildrenCount(element);
      for (int i = 0; i < count; i++)
      {
        var child = VisualTreeHelper.GetChild(element, i);
        foreach (var res in FindVisualChildren<T>(child))
          yield return res;
      }
    }
    public static T FindVisualParent<T>(this DependencyObject element)
      where T : DependencyObject
    {
      while (element != null)
      {
        if (element is T) return (T)element;
        element = VisualTreeHelper.GetParent(element);
      }
      return null;
    }
  }
}