using System;
using System.Collections.Generic;

namespace Yasc.Utils
{
  public static class ListExtensions
  {
    public static int IndexOf<T>(this IList<T> list, Predicate<T> predicate)
    {
      int count = list.Count;
      for (int i = 0; i < count; i++)
        if (predicate(list[i]))
          return i;
      return -1;
    }
  }
  public class EmptyList<T>
  {
    public static readonly List<T> Instance = new List<T>();
  }
}