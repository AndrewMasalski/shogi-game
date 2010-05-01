using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Linq;

namespace Yasc.BoardControl.GenericDragDrop
{
  public static class VisualTreeUtils
  {
    public static T FindAncestor<T>(this DependencyObject obj)
      where T : DependencyObject
    {
      var res = obj as T;
      while (obj != null && res == null)
      {
        obj = VisualTreeHelper.GetParent(obj);
        res = obj as T;
      }
      return res;
    }
    public static T FindAncestor<T>(this DependencyObject obj, Predicate<T> predicate)
      where T : DependencyObject
    {
      var res = obj as T;
      while (obj != null &&(res == null || !predicate(res)))
      {
        obj = VisualTreeHelper.GetParent(obj);
        res = obj as T;
      }
      return res;
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


    public static T FindChild<T>(this DependencyObject obj, Func<T, bool> predicate)
      where T : DependencyObject
    {
      int counter = 0;
      T res = null;
      foreach (var child in FindChildren<T>(obj).Where(predicate))
      {
        if (counter == 0) res = child;
        if (counter == 1) throw new Exception("There are more than one fitting child");
        counter++;
      }
      return res;
    }

    public static T FindLastChild<T>(this DependencyObject obj, Func<T, bool> predicate)
      where T : DependencyObject
    {
      return FindChildren<T>(obj).Where(predicate).LastOrDefault();
    }

    public static IEnumerable<T> FindChildren<T>(this DependencyObject obj)
      where T : DependencyObject
    {
      if (obj == null) yield break;
      var count = VisualTreeHelper.GetChildrenCount(obj);
      for (int i = 0; i < count; i++)
      {
        var child = VisualTreeHelper.GetChild(obj, i);
        if (child is T) yield return (T)child;
        foreach (var c in FindChildren<T>(child))
          yield return c;
      }
    }
  }
}