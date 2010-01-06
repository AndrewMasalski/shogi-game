using System;

namespace DotUsi
{
  /// <summary>Lets engine know how much time white have for the rest of the game</summary>
  public class WhiteTimeModifier : TimeSpanModifier
  {
    /// <summary>Creates modifier for white has <param name="value"> left on the clock</summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> &lt; 0</exception>
    public WhiteTimeModifier(TimeSpan value) 
      : base(value)
    {
      if (value < TimeSpan.Zero)
        throw new ArgumentOutOfRangeException("value", "must not be negative");
    }

    protected override string GetCommandName()
    {
      return "wtime";
    }
  }
}