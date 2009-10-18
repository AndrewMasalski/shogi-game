using System;
using System.Collections.Specialized;

namespace Yasc.Utils
{
  public class ObservableCollectionLocker : IDisposable
  {
    private readonly INotifyCollectionChanged _collection;
    private readonly Flag _changesAllowed = new Flag();

    public ObservableCollectionLocker(INotifyCollectionChanged collection)
    {
      if (collection == null) throw new ArgumentNullException("collection");
      _collection = collection;
      _collection.CollectionChanged += CollectionOnCollectionChanged;
    }

    private void CollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
      if (!_changesAllowed)
        throw new InvalidOperationException(
          "It's not allowed to modify the collection without special handler. " +
          "If you getting this exception it might mean that you are just not allowed to modify it.");
    }

    public IDisposable AllowModifications()
    {
      return _changesAllowed.Set();
    }
    public void Dispose()
    {
      _collection.CollectionChanged -= CollectionOnCollectionChanged;
    }
  }
}