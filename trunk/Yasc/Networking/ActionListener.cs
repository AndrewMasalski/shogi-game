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
      if (_synch == null) 
        _action(obj);
      else
        _synch.Send(p => _action(obj), null);
    }
    public static implicit operator Action<T>(ActionListener<T> l)
    {
      return l.Act;
    }
  }
  /// <summary>This class is needed because delegate target object must be 
  /// <see cref="MarshalByRefObject"/> and target method must be public</summary>
  public class FuncListener<T, TResult> : MarshalByRefObject
  {
    private readonly Func<T, TResult> _action;
    private readonly SynchronizationContext _synch;

    public FuncListener(Func<T, TResult> action)
    {
      _synch = SynchronizationContext.Current;
      _action = action;
    }

    public TResult Act(T obj)
    {
      var res = default(TResult);
      _synch.Send(p => res = _action(obj), null);
      return res;
    }

    public static implicit operator Func<T, TResult>(FuncListener<T, TResult> l)
    {
      return l.Act;
    }
  }
}