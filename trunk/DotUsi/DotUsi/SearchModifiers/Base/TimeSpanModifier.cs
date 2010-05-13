using System;

namespace Yasc.DotUsi.SearchModifiers.Base
{
  /// <summary>Base class for all search modifiers with scalar value of type <see cref="TimeSpan"/></summary>
  public abstract class TimeSpanModifier : ScalarModifier<TimeSpan>
  {
    /// <summary>ctor</summary>
    protected TimeSpanModifier(TimeSpan value) 
      : base(value)
    {
    }
    /// <summary>Override to define how to convert <see cref="ScalarModifier{T}.Value"/> to string</summary>
    protected override string ValueToString()
    {
      return Value.TotalMilliseconds.ToString();
    }
  }
}