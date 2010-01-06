using System;

namespace DotUsi
{
  /// <summary>http://en.wikipedia.org/wiki/Byoyomi</summary>
  /// <remarks>It's not from USI specification. Seen in Shogidokoro</remarks>
  public class ByoyomiModifier : TimeSpanModifier
  {
    public ByoyomiModifier(TimeSpan value)
      : base(value)
    {
    }

    protected override string GetCommandName()
    {
      return "byoyomi";
    }
  }
}