namespace DotUsi
{
  public class SearchMateModifier : UsiSearchModifier
  {
    public string N { get; set; }

    /// <summary>Search for a mate in <paramref name="n"/> moves</summary>
    public SearchMateModifier(string n)
    {
      N = n;
    }

    public override string ToString()
    {
      return "go mate " + N;
    }
  }
}