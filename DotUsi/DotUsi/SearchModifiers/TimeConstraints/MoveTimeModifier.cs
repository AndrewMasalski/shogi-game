using System;

namespace DotUsi
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

    protected override string GetCommandName()
    {
      return "movetime";
    }
  }
}