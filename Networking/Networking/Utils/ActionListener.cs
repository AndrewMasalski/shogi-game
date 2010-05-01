using System;
using System.Threading;

namespace Yasc.Networking.Utils
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
    public static implicit operator Action<T>(ActionListener<T> actionListener)
    {
      return actionListener.Act;
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
      if (_synch == null)  return _action(obj);
      
      var res = default(TResult);
      Exception ex = null;
      _synch.Send(delegate
                    {
                      try
                      {
                        res = _action(obj);
                      }
                      catch (Exception x)
                      {
                        ex = x;
                      }
                    }, null);
      if (ex != null) throw ex;
      return res;
    }

    public static implicit operator Func<T, TResult>(FuncListener<T, TResult> funcListener)
    {
      return funcListener.Act;
    }
  }
}