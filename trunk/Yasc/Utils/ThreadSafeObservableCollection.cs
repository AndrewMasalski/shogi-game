using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Threading;

namespace Yasc.Utils
{
  ///<summary>A class deriving from ObservableCollection to handle cross threads collection changed events</summary>
  ///<typeparam name="T"></typeparam>
  public class ThreadSafeObservableCollection<T> : ObservableCollection<T>
  {
    ///<summary>Occurs when an item is added, removed, changed, moved, or the entire list is refreshed.</summary>
    public override event NotifyCollectionChangedEventHandler CollectionChanged;

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      // Be nice - use BlockReentrancy like MSDN said
      using (BlockReentrancy())
      {
        NotifyCollectionChangedEventHandler eventHandler = CollectionChanged;
        if (eventHandler == null)
          return;

        Delegate[] delegates = eventHandler.GetInvocationList();
        // Walk thru invocation list
        foreach (NotifyCollectionChangedEventHandler handler in delegates)
        {
          var dispatcherObject = handler.Target as DispatcherObject;
          // If the subscriber is a DispatcherObject and different thread
          if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
          {
            // Invoke handler in the target dispatcher's thread
            dispatcherObject.Dispatcher.Invoke(DispatcherPriority.DataBind, handler, this, e);
          }
          else // Execute handler as is
            handler(this, e);
        }
      }
    }
  }
}