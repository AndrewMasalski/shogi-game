using System;
using System.Collections.Generic;
using System.Linq;

namespace Yasc.Utils
{
  public static class ReadOnlyExtensions
  {
    public static ReadOnlyDictionary<TKey, TValue> ToReadOnlyDictionary<T, TKey, TValue>(this IEnumerable<T> source, Func<T, TKey> keySelector, Func<T, TValue> elementSelector)
    {
      return new ReadOnlyDictionary<TKey, TValue>(source.ToDictionary(keySelector, elementSelector));
    }
  }
}