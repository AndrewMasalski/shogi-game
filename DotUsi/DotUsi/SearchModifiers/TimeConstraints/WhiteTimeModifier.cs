using System;

namespace DotUsi
{
  /// <summary>White has x milliseconds left on the clock</summary>
  public class WhiteTimeModifier : TimeSpanModifier
  {
    public WhiteTimeModifier(TimeSpan value) 
      : base(value)
    {
    }

    protected override string GetCommandName()
    {
      return "wtime";
    }
  }
}