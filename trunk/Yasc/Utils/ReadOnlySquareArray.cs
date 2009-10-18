using System;
using System.Collections;
using System.Collections.Generic;

namespace Yasc.Utils
{
  public class ReadOnlySquareArray<T> : IList<T>, IList, ICloneable 
  {
    private readonly T[,] _array;

    public ReadOnlySquareArray(T[,] array)
    {
      _array = array;
    }

    public T this[int x, int y]
    {
      get { return _array[x, y]; }
    }
    object ICloneable.Clone()
    {
      return _array.Clone();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
      return _array.GetEnumerator();
    }
    public IEnumerator<T> GetEnumerator()
    {
      foreach (T item in _array)
        yield return item;
    }
    void ICollection.CopyTo(Array array, int index)
    {
      throw new RankException("The source array is multidimentional");
    }
    void ICollection<T>.CopyTo(T[] array, int arrayIndex)
    {
      throw new RankException("The source array is multidimentional");
    }
    int ICollection<T>.Count
    {
      get { return _array.Length; }
    }
    int ICollection.Count
    {
      get { return _array.Length; }
    }
    object ICollection.SyncRoot
    {
      get { return _array.SyncRoot; }
    }
    bool ICollection.IsSynchronized
    {
      get { return _array.IsSynchronized; }
    }
    int IList.Add(object value)
    {
      throw new NotSupportedException("Add method is not supported on read-only collection");
    }
    void ICollection<T>.Add(T item)
    {
      throw new NotSupportedException("Add method is not supported on read-only collection");
    }
    bool IList.Contains(object value)
    {
      return ((IList) _array).Contains(value);
    }
    bool ICollection<T>.Contains(T item)
    {
      return ((IList)_array).Contains(item);
    }
    void IList.Clear()
    {
      throw new NotSupportedException("Clear method is not supported on read-only collection");
    }
    void ICollection<T>.Clear()
    {
      throw new NotSupportedException("Clear method is not supported on read-only collection");
    }
    int IList.IndexOf(object value)
    {
      throw new RankException("The array is multidimentional");
    }
    int IList<T>.IndexOf(T item)
    {
      throw new RankException("The array is multidimentional");
    }
    void IList.Insert(int index, object value)
    {
      throw new NotSupportedException("Insert method is not supported on read-only collection");
    }
    void IList.Remove(object value)
    {
      throw new NotSupportedException("Remove method is not supported on read-only collection");
    }
    bool ICollection<T>.Remove(T item)
    {
      throw new NotSupportedException("Remove method is not supported on read-only collection");
    }
    void IList<T>.Insert(int index, T item)
    {
      throw new NotSupportedException("Insert method is not supported on read-only collection");
    }
    void IList<T>.RemoveAt(int index)
    {
      throw new NotSupportedException("RemoveAt method is not supported on read-only collection");
    }
    T IList<T>.this[int index]
    {
      get { throw new RankException("The array is multidimentional"); }
      set { throw new RankException("The array is multidimentional"); }
    }
    void IList.RemoveAt(int index)
    {
      throw new NotSupportedException("RemoveAt method is not supported on read-only collection");
    }
    object IList.this[int index]
    {
      get { throw new RankException("The array is multidimentional"); }
      set { throw new RankException("The array is multidimentional"); }
    }
    bool IList.IsReadOnly
    {
      get { return true; }
    }
    bool ICollection<T>.IsReadOnly
    {
      get { return true; }
    }
    bool IList.IsFixedSize
    {
      get { return true; }
    }
  }
}