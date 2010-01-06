using System;

namespace DotUsi
{
  /// <summary>Lets engine know how much time black have for the rest of the game</summary>
  public class BlackTimeModifier : TimeSpanModifier
  {
    /// <summary>Creates modifier for black has <param name="value"> left on the clock</summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> &lt; 0</exception>
    public BlackTimeModifier(TimeSpan value) 
      : base(value)
    {
      if (value < TimeSpan.Zero)
        throw new ArgumentOutOfRangeException("value", "must not be negative");
    }

    protected override string GetCommandName()
    {
      return "btime";
    }
  }
}