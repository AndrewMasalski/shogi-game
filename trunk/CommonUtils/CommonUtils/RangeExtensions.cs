using System.Collections.Generic;

namespace Yasc.Utils
{
  public static class RangeExtensions
  {
    public static Range<T> Range<T>(this IList<T> list)
    {
      return new Range<T>(list);
    }
    public static Range<T> Range<T>(this IList<T> list, int startIndex)
    {
      return new Range<T>(list, startIndex, list.Count - startIndex);
    }
    public static Range<T> Range<T>(this IList<T> list, int startIndex, int width)
    {
      return new Range<T>(list, startIndex, width);
    }
  }
}