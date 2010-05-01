using System;
using DotUsi.SearchModifiers.Base;

namespace DotUsi.SearchModifiers.DepthConstraints
{
  /// <summary>Constraints nodes to search.</summary>
  public class NodesConstraint : ScalarModifier<int>
  {
    /// <summary>Search <param name="count"/> nodes only.</summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> &lt;= 0</exception>
    public NodesConstraint(int count) 
      : base(count)
    {
      if (count <= 0)
        throw new ArgumentOutOfRangeException("count", "must be greater than zero");
    }
    /// <summary>Override to define what to pass to engine as a name of option in 'setoption' command</summary>
    protected override string GetCommandName()
    {
      return "nodes";
    }
  }
}