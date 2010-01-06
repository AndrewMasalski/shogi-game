namespace DotUsi
{
  public class SearchMovesModifier : ScalarModifier<string[]>
  {
    public string[] Moves { get; set; }

    /// <summary>Restrict search to <paramref name="moves"/> only.</summary>
    /// <example>After <code>
    ///   Position(new string[0]);
    ///   GoSearchMoves(new []{"7g7f", "2g2f"});</code>
    /// The engine will only search (choose between) P-7f and P-2f 
    /// moves in the initial position. </example>
    public SearchMovesModifier(params string[] moves)
      : base(moves)
    {
    }
    protected override string ValueToString()
    {
      return string.Join(" ", Moves);
    }
    protected override string GetCommandName()
    {
      return "searchmoves";
    }
  }
}