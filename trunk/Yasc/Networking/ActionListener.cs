using System;

namespace Yasc.Networking
{
  /// <summary>This class is needed because delegate target object must be 
  /// <see cref="MarshalByRefObject"/> and target method must be public</summary>
  public class ActionListener<T> : MarshalByRefObject
  {
    private readonly Action<T> _action;

    public ActionListener(Action<T> action)
    {
      _action = action;
    }

    public void Act(T obj)
    {
      _action(obj);
    }
    public static implicit operator Action<T>(ActionListener<T> l)
    {
      return l.Act;
    }
  }

  /// <summary>This class is needed because delegate target object must be 
  /// <see cref="MarshalByRefObject"/> and target method must be public</summary>
  public class ActionListener : MarshalByRefObject
  {
    private readonly Action _action;

    public ActionListener(Action action)
    {
      _action = action;
    }

    public void Act()
    {
      _action();
    }
    public static implicit operator Action(ActionListener l)
    {
      return l.Act;
    }
  }
}