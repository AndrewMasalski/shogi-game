using System;

namespace DotUsi
{
  /// <summary><para>Byo-yomi is an extended time control in two-player games, specifically Shogi and Go.
  ///  The word is borrowed from Japanese, where it additionally means "countdown" in general.</para>
  ///  <para>A typical time control is "60 minutes + 30 seconds byo-yomi", 
  ///   which means that each player may make as many or as few moves as he chooses 
  ///   during his first 60 minutes of thinking time, but after the hour is exhausted, 
  ///   he must make each move in thirty seconds or less</para>
  /// </summary>
  /// <seealso cref="http://en.wikipedia.org/wiki/Byoyomi"/>
  /// <remarks>It's not from USI specification. Seen in Shogidokoro</remarks>
  public class ByoyomiModifier : TimeSpanModifier
  {
    /// <summary>Creates modifier for byo-yomi set for the game</summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> &lt; 0</exception>
    public ByoyomiModifier(TimeSpan value)
      : base(value)
    {
      if (value < TimeSpan.Zero)
        throw new ArgumentOutOfRangeException("value", "must not be negative");
    }

    protected override string GetCommandName()
    {
      return "byoyomi";
    }
  }
}