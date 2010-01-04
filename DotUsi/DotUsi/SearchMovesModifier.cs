namespace DotUsi
{
  public class SearchMovesModifier : UsiSearchModifier
  {
    public string[] Moves { get; set; }

    /// <summary>Restrict search to <paramref name="moves"/> only.</summary>
    /// <example>After <code>
    ///   Position(new string[0]);
    ///   GoSearchMoves(new []{"7g7f", "2g2f"});</code>
    /// The engine will only search (choose between) P-7f and P-2f 
    /// moves in the initial position. </example>
    public SearchMovesModifier(params string[] moves)
    {
      Moves = moves;
    }

    public override string ToString()
    {
      return "searchmoves " + string.Join(" ", Moves);
    }
  }
}