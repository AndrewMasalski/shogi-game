using System;

namespace DotUsi
{
  /// <summary>White time increment per move</summary>
  public class WhiteIncrementModifier : TimeSpanModifier
  {
    /// <summary>Creates modifier for white increment per move is <param name="value"/></summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> &lt; 0</exception>
    public WhiteIncrementModifier(TimeSpan value) 
      : base(value)
    {
      if (value < TimeSpan.Zero)
        throw new ArgumentOutOfRangeException("value", "must not be negative");
    }

    protected override string GetCommandName()
    {
      return "winc";
    }
  }
}