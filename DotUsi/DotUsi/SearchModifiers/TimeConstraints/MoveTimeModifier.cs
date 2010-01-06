using System;

namespace DotUsi
{
  /// <summary>Search exactly t milliseconds</summary>
  public class MoveTimeModifier : TimeSpanModifier
  {
    public MoveTimeModifier(TimeSpan value) 
      : base(value)
    {
    }

    protected override string GetCommandName()
    {
      return "movetime";
    }
  }
}