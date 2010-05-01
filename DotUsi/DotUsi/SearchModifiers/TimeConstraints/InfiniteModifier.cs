using DotUsi.SearchModifiers.Base;

namespace DotUsi.SearchModifiers.TimeConstraints
{
  /// <summary>Search until the stop command is received. 
  ///   Do not exit the search without being told so in this mode!
  /// </summary>
  public class InfiniteModifier : UsiSearchModifier
  {
    /// <summary>Override to define what to pass to engine as a part of the 'setoption' command</summary>
    protected override string GetCommand()
    {
      return "infinite";
    }
  }
}