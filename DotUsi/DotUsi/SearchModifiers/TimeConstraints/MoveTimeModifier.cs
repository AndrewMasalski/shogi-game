using System;
using Yasc.DotUsi.SearchModifiers.Base;

namespace Yasc.DotUsi.SearchModifiers.TimeConstraints
{
  /// <summary>Sets exact time fir search</summary>
  public class MoveTimeModifier : TimeSpanModifier
  {
    /// <summary>Creates modifier to search exactly <paramref name="value"/> time</summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> &lt;= 0</exception>
    public MoveTimeModifier(TimeSpan value) 
      : base(value)
    {
      if (value <= TimeSpan.Zero)
        throw new ArgumentOutOfRangeException("value", "must not greater than zero");
    }

    /// <summary>Override to define what to pass to engine as a name of option in 'setoption' command</summary>
    protected override string GetCommandName()
    {
      return "movetime";
    }
  }
}