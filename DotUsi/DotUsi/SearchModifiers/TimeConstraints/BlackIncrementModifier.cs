using System;

namespace DotUsi
{
  /// <summary>Black time increment per move</summary>
  public class BlackIncrementModifier : TimeSpanModifier
  {
    /// <summary>Creates modifier for black increment per move is <param name="value"/></summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> &lt; 0</exception>
    public BlackIncrementModifier(TimeSpan value) 
      : base(value)
    {
      if (value < TimeSpan.Zero)
        throw new ArgumentOutOfRangeException("value", "must not be negative");
    }

    protected override string GetCommandName()
    {
      return "binc";
    }
  }
}