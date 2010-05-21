using System;
using System.Collections;
using System.Collections.Generic;

namespace Yasc.Utils
{
  public static class EnumerableExtensions
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
          result = (result * 397) ^ (item == null ? 0 : item.GetHashCode());
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
    public static IEnumerable<Tuple<T, T>> Pairs<T>(this IEnumerable<T> list)
    {
      var i = 0;
      var first = default(T);
      foreach (var item in list)
      {
        if (++i % 2 == 1)
          first = item;
        else
          yield return Tuple.Create(first, item);
      }
    }
    public static IEnumerable<Tuple<T1, T2>> Pairs<T1, T2>(this IEnumerable<T2> list, Converter<T2, T1> firstConverter)
    {
      var i = 0;
      var first = default(T2);
      foreach (var item in list)
      {
        if (++i % 2 == 1)
          first = item;
        else
          yield return Tuple.Create(firstConverter(first), item);
      }
    }
    public static IEnumerable<Tuple<T1, T2>> Pairs<T1, T2>(this IEnumerable<T1> list, Converter<T1, T2> secondConverter)
    {
      var i = 0;
      var first = default(T1);
      foreach (var item in list)
      {
        if (++i % 2 == 1)
          first = item;
        else
          yield return Tuple.Create(first, secondConverter(item));
      }
    }
    public static IEnumerable<Tuple<T1, T2>> Pairs<T, T1, T2>(this IEnumerable<T> list, Converter<T, T1> firstConverter, Converter<T, T2> secondConverter)
    {
      var i = 0;
      var first = default(T1);
      foreach (var item in list)
      {
        if (++i % 2 == 1)
          first = firstConverter(item);
        else
          yield return Tuple.Create(first, secondConverter(item));
      }
    }
    public static IEnumerable<TResult> Triples<TSource, TResult>(this IEnumerable<TSource> list, Func<TSource, TSource, TSource, TResult> converter)
    {
      using (var enumerator = list.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          var a1 = enumerator.Current;
          if (!enumerator.MoveNext()) throw new NotInOrderOfException(3);
          var a2 = enumerator.Current;
          if (!enumerator.MoveNext()) throw new NotInOrderOfException(3);
          var a3 = enumerator.Current;
          yield return converter(a1, a2, a3);
        }
      }
    }
  }

  public class NotInOrderOfException : Exception
  {
    public NotInOrderOfException(int order)
      : base("Source collection is not in order of " + order)
    {
    }
  }
}