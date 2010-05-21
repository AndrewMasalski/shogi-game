using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Yasc.Utils
{
  public struct Range<T> : IEnumerable<T>
  {
    public bool IsEmpty
    {
      get { return List == null || Width == 0; }
    }
    public IList<T> List { get; private set; }
    public int Index { get; private set; }
    public int Width { get; private set; }

    public Range(IList<T> list)
      : this(list, 0, list.Count)
    {
    }
    public Range(IList<T> list, int index, int width)
      : this()
    {
      if (list == null) throw new ArgumentNullException("list");
      if (width == 0) throw new ArgumentOutOfRangeException("width");
      if (index < 0 || index >= list.Count) throw new ArgumentOutOfRangeException("index");
      if (index + width > list.Count) throw new ArgumentOutOfRangeException("width", "index + width > list.Count");
      List = list;
      Index = index;
      Width = width;
    }

    public Range<T> EmptySubrange()
    {
      return new Range<T>();
    }
    public Range<T> MiddleSubrange(int size)
    {
      if (size > Width) throw new ArgumentOutOfRangeException("size", "cannot be large than Width");
      if (size < 0) throw new ArgumentOutOfRangeException("size", "cannot be negative");
      return new Range<T>(List, Index + (Width - size) / 2, size);
    }

    public Range<T> Find<TSrc>(Range<TSrc> range, Comparer<T, TSrc> comparer)
    {
      if (Width < range.Width) throw new ArgumentOutOfRangeException("range", "is larger than me");
      for (int i = 0; i <= Width - range.Width; i++)
      {
        int j = 0;
        for (; j < range.Width; j++)
        {
          if (!comparer(this[i + j], range[j]))
          {
            break;
          }
        }
        if (j == range.Width)
          return new Range<T>(List, i, range.Width);
      }
      return new Range<T>();
    }

    public T this[int i]
    {
      get { return List[Index + i]; }
    }

    public IEnumerable<Range<T>> Break(Range<T> range)
    {
      if (range.IsEmpty) throw new ArgumentOutOfRangeException("range", "cannot be empty");
      if (!IsSubrange(range)) throw new ArgumentOutOfRangeException("range", "must be subrange");
      return BreakInternal(range);
    }

    private IEnumerable<Range<T>> BreakInternal(Range<T> range)
    {
      if (range.Width == Width)
        yield break;

      if (range.Index == Index)
      {
        yield return new Range<T>(List, Index + range.Width, Width - range.Width);
      }
      else if (range.UpperBound == UpperBound)
      {
        yield return new Range<T>(List, Index, range.Index - Index);
      }
      else
      {
        int firstPartWidth = range.Index - Index;
        yield return new Range<T>(List, Index, firstPartWidth);
        yield return new Range<T>(List, range.Index + range.Width, Width - range.Width - firstPartWidth);
      }
    }
    public int UpperBound
    {
      get { return Index + Width - 1; }
    }
    private bool IsSubrange(Range<T> range)
    {
      if (range.IsEmpty) return true;
      if (range.Index < Index) return false;
      if (range.UpperBound > UpperBound) return false;
      return true;
    }

    public bool SameAs(Range<T> other)
    {
      return ReferenceEquals(List, other.List) && Index == other.Index && Width == other.Width;
    }
    public bool Equals(Range<T> other)
    {
      if (other.SameAs(this)) return true;

      int count = Width;
      if (other.Width != count) return false;
      
      for (int i = 0; i < count; i++)
        if (!Equals(this[i], other[i]))
          return false;

      return true;
    }

    public IEnumerator<T> GetEnumerator()
    {
      int count = Width;
      for (int i = 0; i < count; i++)
        yield return this[i];
    }
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (obj.GetType() != typeof (Range<T>)) return false;
      return Equals((Range<T>) obj);
    }
    public override int GetHashCode()
    {
      unchecked
      {
        int result = 0;
        foreach (var item in this)
          result = (result * 397) ^ item.GetHashCode();
        return result;
      }
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public override string ToString()
    {
      return "(" + Index + ", " + Width + ") = {" + 
             string.Join(", ", this.Select(o => o.ToString()).ToArray()) + "}" ;
    }
  }
}