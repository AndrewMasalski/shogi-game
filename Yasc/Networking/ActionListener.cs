using System;
using System.Threading;

namespace Yasc.Networking
{
  /// <summary>This class is needed because delegate target object must be 
  /// <see cref="MarshalByRefObject"/> and target method must be public</summary>
  public class ActionListener<T> : MarshalByRefObject
  {
    private readonly Action<T> _action;
    private readonly SynchronizationContext _synch;

    public ActionListener(Action<T> action)
    {
      _synch = SynchronizationContext.Current;
      _action = action;
    }

    public void Act(T obj)
    {
      _synch.Send(p => _action(obj), null);
    }
    public static implicit operator Action<T>(ActionListener<T> l)
    {
      return l.Act;
    }
  }
}