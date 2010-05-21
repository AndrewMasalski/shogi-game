using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Yasc.Utils
{
  public static class ObservableCollectionExtensions
  {
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
    private static HashSet<object> GetSet<T>(IEnumerable<T> src, Converter<T, object> converter)
    {
      var res = new HashSet<object>();
      foreach (var item in src)
        if (!res.Add(converter(item)))
          return null;
      return res;
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
}