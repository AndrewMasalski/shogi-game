using System;

namespace DotUsi
{
  /// <summary>There are n moves to the next time control.</summary>
  /// <remarks>If you don't get this and get the <see cref="WhiteTimeModifier"/> 
  ///   and <see cref="BlackTimeModifier"/>, it's sudden death. ?
  /// </remarks>
  public class MovesToGoModifier : ScalarModifier<int>
  {
    /// <summary>There are <paramref name="value"/> moves to the next time control.</summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> &lt;= 0</exception>
    public MovesToGoModifier(int value) 
      : base(value)
    {
      if (value <= 0)
        throw new ArgumentOutOfRangeException("value", "must be greater than 0");
    }

    protected override string GetCommandName()
    {
      return "movestogo";
    }
  }
}