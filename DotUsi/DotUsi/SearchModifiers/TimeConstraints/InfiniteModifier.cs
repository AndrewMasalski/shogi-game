namespace DotUsi
{
  /// <summary>Search until the stop command is received. 
  ///   Do not exit the search without being told so in this mode!
  /// </summary>
  public class InfiniteModifier : UsiSearchModifier
  {
    protected override string GetCommand()
    {
      return "infinite";
    }
  }
}