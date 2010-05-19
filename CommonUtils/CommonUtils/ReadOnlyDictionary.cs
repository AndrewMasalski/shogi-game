using System;
using System.Collections;
using System.Collections.Generic;

namespace Yasc.Utils
{
  public sealed class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
  {
    private readonly IDictionary<TKey, TValue> _dictionary;
    private readonly bool _isFixedSize;

    public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
      : this(dictionary, true)
    {
    }

    public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary, bool makeCopy)
    {
      _dictionary = makeCopy ? new Dictionary<TKey, TValue>(dictionary) : dictionary;
      _isFixedSize = makeCopy;
    }

    public bool IsFixedSize
    {
      get { return _isFixedSize; }
    }
    public int Count
    {
      get { return _dictionary.Count; }
    }
    public bool IsReadOnly
    {
      get { return true; }
    }
    public TValue this[TKey key]
    {
      get { return _dictionary[key]; }
      set { throw new InvalidOperationException("ObjectIsReadOnly"); }
    }
    public ICollection<TKey> Keys
    {
      get { return _dictionary.Keys; }
    }
    public ICollection<TValue> Values
    {
      get { return _dictionary.Values; }
    }
    public bool TryGetValue(TKey key, out TValue value)
    {
      return _dictionary.TryGetValue(key, out value);
    }
    public bool ContainsKey(TKey key)
    {
      return _dictionary.ContainsKey(key);
    }

    #region ' Explicit IDictionary<TKey, TValue> Members '

    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
    {
      throw new InvalidOperationException("ObjectIsReadOnly");
    }
    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
    {
      if (ContainsKey(keyValuePair.Key))
      {
        TValue local = this[keyValuePair.Key];
        return local.Equals(keyValuePair.Value);
      }
      return false;
    }
    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      _dictionary.CopyTo(array, arrayIndex);
    }
    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
    {
      throw new InvalidOperationException("ObjectIsReadOnly");
    }
    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
      return _dictionary.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable<KeyValuePair<TKey, TValue>>) this).GetEnumerator();
    }


    void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
    {
      throw new InvalidOperationException("ObjectIsReadOnly");
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Clear()
    {
      throw new InvalidOperationException("ObjectIsReadOnly");
    }
    bool IDictionary<TKey, TValue>.Remove(TKey key)
    {
      throw new InvalidOperationException("ObjectIsReadOnly");
    }

    #endregion
  }
}