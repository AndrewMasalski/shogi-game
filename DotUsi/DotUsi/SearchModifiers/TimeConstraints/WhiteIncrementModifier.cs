using System;

namespace DotUsi
{
  /// <summary>Black increment per move i milliseconds (if i > 0)</summary>
  public class WhiteIncrementModifier : TimeSpanModifier
  {
    public WhiteIncrementModifier(TimeSpan value) 
      : base(value)
    {
    }

    protected override string GetCommandName()
    {
      return "winc";
    }
  }
}