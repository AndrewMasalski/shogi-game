using System;
using System.Collections;
using System.Collections.Generic;

namespace Yasc.Utils
{
  public static class ListUtils
  {
    public static int CalcHashCode(this IEnumerable list)
    {
      unchecked
      {
        int result = 0;
        foreach (var item in list)
          result = result ^ (item == null ? 0 : item.GetHashCode());
        return result;
      }
    }    
    public static int GetSeqHashCode(this IEnumerable list)
    {
      unchecked
      {
        int result = 0;
        foreach (var item in list)
          result = (result*397) ^ (item == null ? 0 : item.GetHashCode());
        return result;
      }
    }
    public static int CalcHashCode(params int[] list)
    {
      return list.CalcHashCode();
    }
    public static int GetSeqHashCode(params int[] list)
    {
      return list.GetSeqHashCode();
    }

    public static bool Equal(IEnumerable first, IEnumerable second)
    {
      IEnumerator ae = first.GetEnumerator();
      IEnumerator be = second.GetEnumerator();
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

    public static void Update<TDestination, TSource>(
      this IList<TDestination> list, 
      IEnumerable<TSource> src, 
      Comparer<TDestination, TSource> comparer, 
      Converter<TSource, TDestination> converter)
    {
      list.Clear();
      foreach (var item in src)
        list.Add(converter(item));
    }
  }

  public delegate bool Comparer<T, U>(T t, U u);
}