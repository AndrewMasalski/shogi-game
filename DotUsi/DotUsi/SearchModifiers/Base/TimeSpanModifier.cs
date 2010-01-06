using System;

namespace DotUsi
{
  public abstract class TimeSpanModifier : ScalarModifier<TimeSpan>
  {
    protected TimeSpanModifier(TimeSpan value) 
      : base(value)
    {
    }
    protected override string ValueToString()
    {
      return Value.TotalMilliseconds.ToString();
    }
  }
}