using System;
using System.Collections;
using System.Collections.Generic;

namespace Yasc.Utils
{
  public static class ListUtils
  {
    public static int GetHashcode(this IEnumerable list)
    {
      unchecked
      {
        int result = 0;
        foreach (var item in list)
          result = result ^ (item == null ? 0 : item.GetHashCode());
        return result;
      }
    }    
    public static int GetSeqHashcode(this IEnumerable list)
    {
      unchecked
      {
        int result = 0;
        foreach (var item in list)
          result = (result*397) ^ (item == null ? 0 : item.GetHashCode());
        return result;
      }
    }
    public static int GetHashcode(params int[] list)
    {
      return list.GetHashcode();
    }
    public static int GetSeqHashcode(params int[] list)
    {
      return list.GetSeqHashcode();
    }

    public static bool Equal(IEnumerable a, IEnumerable b)
    {
      throw new NotImplementedException();
    }

    public static bool Equivalent(IEnumerable a, IEnumerable b)
    {
      throw new NotImplementedException();
    }

    public static void Update<T, U>(this IList<T> list, IEnumerable<U> src, Comparer<T, U> comparer, Converter<U, T> converter)
    {
      list.Clear();
      foreach (var item in src)
        list.Add(converter(item));
    }
  }

  public delegate bool Comparer<T, U>(T t, U u);
}