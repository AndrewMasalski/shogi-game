using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Yasc.GenericDragDrop
{
  public static class VisualTreeUtils
  {
    public static T FindAncestor<T>(this DependencyObject obj)
      where T : DependencyObject
    {
      var res = obj as T;
      while (res == null)
      {
        obj = VisualTreeHelper.GetParent(obj);
        res = obj as T;
      }
      return res;
    }
    public static DependencyObject FindAncestor(this DependencyObject obj, Predicate<DependencyObject> predicate)
    {
      while (obj != null && !predicate(obj))
      {
        obj = VisualTreeHelper.GetParent(obj);
      }
      return obj;
    }

    public static T FindChild<T>(this DependencyObject obj)
      where T : DependencyObject
    {
      int counter = 0;
      T res = null;
      foreach (var child in FindChildren<T>(obj))
      {
        if (counter == 0) res = child;
        if (counter == 1) throw new Exception("There are more than one fitting child");
        counter++;
      }
      return res;
    }

    public static IEnumerable<T> FindChildren<T>(this DependencyObject obj)
      where T : DependencyObject
    {
      if (obj == null) yield break;
      if (obj is T) yield return (T)obj;
      var count = VisualTreeHelper.GetChildrenCount(obj);
      for (int i = 0; i < count; i++)
      {
        var child = VisualTreeHelper.GetChild(obj, i);
        foreach (var c in FindChildren<T>(child))
          yield return c;
      }
    }
  }
}