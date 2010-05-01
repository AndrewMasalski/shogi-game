using System;
using DotUsi.SearchModifiers.Base;

namespace DotUsi.SearchModifiers.TimeConstraints
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

    /// <summary>Override to define what to pass to engine as a name of option in 'setoption' command</summary>
    protected override string GetCommandName()
    {
      return "winc";
    }
  }
}