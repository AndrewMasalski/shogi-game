using System;

namespace DotUsi
{
  /// <summary>Constraints search depth.</summary>
  public class DepthConstraint : ScalarModifier<int>
  {
    /// <summary>Search <param name="depth"/> moves only.</summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="depth"/> &lt;= 0</exception>
    public DepthConstraint(int depth)
      : base(depth)
    {
      if (depth <= 0)
        throw new ArgumentOutOfRangeException("depth", "must be greater than 0");
    }

    protected override string GetCommandName()
    {
      return "depth";
    }
  }
}