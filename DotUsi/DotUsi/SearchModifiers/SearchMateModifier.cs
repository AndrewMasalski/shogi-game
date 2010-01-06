namespace DotUsi
{
  /// <summary>Search for a mate in N moves</summary>
  public class SearchMateModifier : ScalarModifier<int>
  {
    public SearchMateModifier(int value) 
      : base(value)
    {
    }

    protected override string GetCommandName()
    {
      return "mate";
    }
  }
}