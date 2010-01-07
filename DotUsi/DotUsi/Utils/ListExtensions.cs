using System.Collections.Generic;

namespace DotUsi
{
  public static class ListExtensions
  {
    public static IEnumerable<KeyValuePair<T, T>> Pairs<T>(this IList<T> list)
    {
      int count = list.Count;
      T key = default(T);
      for (int i = 0; i < count; i++)
      {
        if (i % 2 == 0)
          key = list[i];
        else
          yield return new KeyValuePair<T, T>(key, list[i]);
      }
    }
    public static IEnumerable<T> Range<T>(this IList<T> src, int start, int length)
    {
      for (int i = 0; i < length; i++)
        yield return src[start + i];
    }
  }
}