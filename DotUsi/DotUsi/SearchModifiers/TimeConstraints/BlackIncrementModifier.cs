using System;

namespace DotUsi
{
  /// <summary>Black increment per move i milliseconds (if i > 0)</summary>
  public class BlackIncrementModifier : TimeSpanModifier
  {
    /// <summary>Black increment per move is <param name="value"/> milliseconds</summary>
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