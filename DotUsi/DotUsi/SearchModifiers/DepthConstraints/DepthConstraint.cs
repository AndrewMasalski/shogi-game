using System;
using DotUsi.SearchModifiers.Base;

namespace DotUsi.SearchModifiers.DepthConstraints
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

    /// <summary>Override to define what to pass to engine as a name of option in 'setoption' command</summary>
    protected override string GetCommandName()
    {
      return "depth";
    }
  }
}