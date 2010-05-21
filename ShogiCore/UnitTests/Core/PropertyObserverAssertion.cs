using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.Utils.Mvvm;

namespace ShogiCore.UnitTests.ShogiCore
{
  public class PropertyObserverAssertion<T> 
    where T : INotifyPropertyChanged
  {
    private readonly PropertyObserver<T> _reference;
    private int _id;
    private readonly Dictionary<int, int> 
      _expectations = new Dictionary<int, int>();
    private readonly Dictionary<int, int> 
      _counters = new Dictionary<int, int>();

    public PropertyObserverAssertion(T propertySource)
    {
      _reference = new PropertyObserver<T>(propertySource);
    }

    public PropertyObserverAssertion<T> RegisterCounter(
      Expression<Func<T, object>> expression, int howManyTimes)
    {
      int id = _id++;
      _expectations[id] = howManyTimes;
      _counters[id] = 0;
      _reference.RegisterHandler(expression, o => _counters[id]++);
      return this;
    }

    public void Check()
    {
      foreach (var pair in _expectations)
        Assert.AreEqual(pair.Value, _counters[pair.Key]);
    }
  }
}