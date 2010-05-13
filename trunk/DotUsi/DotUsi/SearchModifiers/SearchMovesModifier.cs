using System;
using System.Linq;
using Yasc.DotUsi.SearchModifiers.Base;

namespace Yasc.DotUsi.SearchModifiers
{
  /// <summary>Restricts search to some branches.</summary>
  public class SearchMovesModifier : ScalarModifier<string[]>
  {
    /// <summary>Restrict search to <paramref name="moves"/> only.</summary>
    /// <example>After <code>
    ///   Position(new string[0]);
    ///   Go(new SearchMovesModifier("7g7f", "2g2f"));</code>
    /// The engine will only search (choose between) P-7f and P-2f 
    /// moves in the initial position. </example>
    public SearchMovesModifier(params string[] moves)
      : base(moves)
    {
      if (moves.Where(m => m == null).Count() > 0)
        throw new ArgumentOutOfRangeException("moves", "null moves are not allowed!");
    }
    /// <summary>Override to define how to convert <see cref="ScalarModifier{T}.Value"/> to string</summary>
    protected override string ValueToString()
    {
      return string.Join(" ", Value);
    }
    /// <summary>Override to define what to pass to engine as a name of option in 'setoption' command</summary>
    protected override string GetCommandName()
    {
      return "searchmoves";
    }
  }
}