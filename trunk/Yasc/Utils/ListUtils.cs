using System;
using System.Collections;
using System.Collections.Generic;

namespace Yasc.Utils
{
  public static class ListUtils
  {
    public static int CalcHashcode(this IEnumerable list)
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
    public static int CalcHashcode(params int[] list)
    {
      return list.CalcHashcode();
    }
    public static int GetSeqHashcode(params int[] list)
    {
      return list.GetSeqHashcode();
    }

    public static bool Equal(IEnumerable a, IEnumerable b)
    {
      IEnumerator ae = a.GetEnumerator();
      IEnumerator be = b.GetEnumerator();
      while (true)
      {
        bool an = ae.MoveNext();
        bool bn = be.MoveNext();
        if (an != bn)
          return false;
        if (!an)
          return true;
        if (!Equals(ae.Current, be.Current))
          return false;
      }
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