using System;

namespace DotUsi
{
  /// <summary>Search for a mate</summary>
  public class SearchMateModifier : ScalarModifier<int>
  {
    /// <summary>Search for a mate in <paramref name="value"/> moves</summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> &lt;= 0</exception>
    public SearchMateModifier(int value) 
      : base(value)
    {
      if (value <= 0)
        throw new ArgumentOutOfRangeException("value", "must be greater than 0");
    }

    protected override string GetCommandName()
    {
      return "mate";
    }
  }
}