using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
    public static int IndexOf<T>(this IList<T> list, Predicate<T> predicate)
    {
      int count = list.Count;
      for (int i = 0; i < count; i++)
        if (predicate(list[i]))
          return i;
      return -1;
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

    public static void Update<T>(this ObservableCollection<T> list, IEnumerable<T> src)
    {
      var currentIndexes = GetSet(list);
      if (currentIndexes == null)
      {
        ClearUpdate(list, src);
        return;
      }
      var newIndexes = GetDic(src);
      if (newIndexes == null)
      {
        ClearUpdate(list, src);
        return;
      }

      int i = 0;
      foreach (var item in src)
      {
        if (i >= list.Count)
        {
          list.Add(item);
          i++;
          continue;
        }
        if (Equals(item, list[i]))
        {
          i++;
          continue;
        }

        // Is there item which should be at position i 
        // in current collection?
        if (currentIndexes.Contains(item))
        {
          var currentIndex = list.IndexOf(item);
          // Can we just remove everything between 
          // current position of that item and new position?
          bool canRemove = true;
          for (int j = i; j < currentIndex; j++)
          {
            if (newIndexes.ContainsKey(list[j]))
            {
              canRemove = false;
              break;
            }
          }
          if (canRemove)
          {
            for (int j = currentIndex - 1; j >= i; j--)
            {
              currentIndexes.Remove(list[j]);
              list.RemoveAt(j);
            }
          }
          else
          {
            // We have to exchange
            list.Move(currentIndex, i);
            // The item we've just arranged goes out of the scope of our interests
            currentIndexes.Remove(item);
          }
          // Now item at position i is justified
        }
        else
        {
          // Item which should be at position i 
          // is not even in current collection!
          
          // Item which is at the position i now
          // Is it in the new collection?
          int newIndex;
          if (newIndexes.TryGetValue(list[i], out newIndex))
          {
            list.Insert(i, item);
          }
          else
          {
            currentIndexes.Remove(list[i]);
            list[i] = item;
          }
        }
        i++;
      }
      while (i < list.Count)
        list.RemoveAt(list.Count - 1);
    }

    private static void ClearUpdate<T>(IList<T> list, IEnumerable<T> src)
    {
      while (list.Count > 0)
        list.RemoveAt(list.Count - 1);
      foreach (var item in src)
        list.Add(item);
    }

    private static void ClearUpdate<T1, T2>(IList<T1> list, IEnumerable<T2> src, Converter<T2, T1> directConverter)
    {
      while (list.Count > 0)
        list.RemoveAt(list.Count - 1);
      foreach (var item in src)
        list.Add(directConverter(item));
    }

    public static void Update<T1, T2>(this ObservableCollection<T1> list, IEnumerable<T2> src, 
      Converter<T1, object> listItemConverter, Converter<T2, object> srcItemConverter, 
      Converter<T2, T1> directConverter)
    {
      var currentIndexes = GetSet(list, listItemConverter);
      if (currentIndexes == null)
      {
        ClearUpdate(list, src, directConverter);
        return;
      }
      var newIndexes = GetDic(src, srcItemConverter);
      if (newIndexes == null)
      {
        ClearUpdate(list, src, directConverter);
        return;
      }

      int i = 0;
      foreach (var item in src)
      {
        if (i >= list.Count)
        {
          list.Add(directConverter(item));
          i++;
          continue;
        }
        object newItemKey = srcItemConverter(item);
        object currItemKey = listItemConverter(list[i]);
        if (Equals(newItemKey, currItemKey))
        {
          i++;
          continue;
        }

        // Is there item which should be at position i 
        // in current collection?
        if (currentIndexes.Contains(newItemKey))
        {
          var currentIndex = list.IndexOf(it => Equals(listItemConverter(it), newItemKey));
          // Can we just remove everything between 
          // current position of that item and new position?
          bool canRemove = true;
          for (int j = i; j < currentIndex; j++)
          {
            if (newIndexes.ContainsKey(listItemConverter(list[j])))
            {
              canRemove = false;
              break;
            }
          }
          if (canRemove)
          {
            for (int j = currentIndex - 1; j >= i; j--)
            {
              list.RemoveAt(j);
            }
          }
          else
          {
            // We have to move
            list.Move(currentIndex, i);
          }
          // Now item at position i is justified
        }
        else
        {
          // Item which should be at position i 
          // is not even in current collection!
          
          // Item which is at the position i now
          // Is it in the new collection?
          int newIndex;
          if (newIndexes.TryGetValue(currItemKey, out newIndex))
          {
            list.Insert(i, directConverter(item));
          }
          else
          {
            list[i] = directConverter(item);
          }
        }
        i++;
      }
      while (i < list.Count)
        list.RemoveAt(list.Count - 1);
    }

    private static HashSet<object> GetSet<T>(IEnumerable<T> src, Converter<T, object> converter)
    {
      var res = new HashSet<object>();
      foreach (var item in src)
        if (!res.Add(converter(item)))
          return null;
      return res;
    }

    private static Dictionary<T, int> GetDic<T>(IEnumerable<T> src)
    {
      int i = 0;
      var res = new Dictionary<T, int>();
      foreach (var item in src)
      {
        if (res.ContainsKey(item))
          return null;
        res[item] = i++;
      }
      return res;
    }
    private static Dictionary<object, int> GetDic<T>(IEnumerable<T> src, Converter<T, object> converter)
    {
      int i = 0;
      var res = new Dictionary<object, int>();
      foreach (var item in src)
      {
        var key = converter(item);
        if (res.ContainsKey(key))
          return null;
        res[key] = i++;
      }
      return res;
    }
    private static HashSet<T> GetSet<T>(IEnumerable<T> src)
    {
      var res = new HashSet<T>();
      foreach (var item in src)
        if (!res.Add(item))
          return null;
      return res;
    }
  }

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
  public delegate bool Comparer<T1, T2>(T1 t, T2 u);

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