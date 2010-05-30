namespace Yasc.ShogiCore.Moves
{
  /// <summary>
  /// http://en.wikipedia.org/wiki/Punctuation_(chess)#Alternate_uses
  /// </summary>
  public enum MoveEvaluation
  {
    /// <summary>
    /// <para>??: Blunder</para>
    /// The double question mark "??" indicates a blunder, a very bad mistake. Typical moves which 
    /// receive double question marks are those that overlook that the queen is under attack or overlook 
    /// a checkmate. Whether a single or double question mark is used often depends on the player's 
    /// strength. For instance, if a beginner makes a serious strategic error (for instance, allowing 
    /// doubled pawns or exchanging into a lost endgame) or overlooks a tactical sequence, 
    /// this might be explained by the beginner's lack of skill, and be given only one question mark. 
    /// If a master were to make the same move, some annotators might use the double question mark 
    /// to indicate that one would never expect a player of the master's strength to make such a weak move.
    /// </summary>
    Blunder,
    /// <summary>
    /// <para>?: Mistake</para>
    /// A single question mark "?" after a move indicates that the annotator thinks that the move is a 
    /// poor one that should not be played. However, the nature of the mistake may be more strategic 
    /// than tactical in nature; or, in some cases, the move receiving a question mark may be one that 
    /// is difficult to find a refutation for.
    /// </summary>
    Mistake,
    /// <summary>
    /// <para>?!: Dubious move</para>
    /// This symbol is similar to the "!?" (below) but usually indicates that the annotator believes
    /// the move to be objectively bad, albeit hard to refute. The "?!" is also often used instead 
    /// of a "?" to indicate that the move is not all bad. A sacrifice leading to a dangerous attack 
    /// which the opponent should be able to defend against if he plays well may receive a "?!". 
    /// Alternatively, this may denote a move that is truly bad, but sets up an attractive trap.
    /// </summary>
    Dubious,
    /// <summary>
    /// <para>!?: Interesting move</para>
    /// The "!?" is one of the more controversial symbols. Different books have slightly varying 
    /// definitions. Among the definitions are "interesting, but perhaps not the best move", "move 
    /// deserving attention", "enterprising move" and "risky move". Usually it indicates that the 
    /// move leads to exciting or wild play and that the move is probably good. It is also often 
    /// used when a player sets a cunning trap in a lost position. Typical moves receiving a "!?" 
    /// are those involving speculative sacrifices or dangerous attacks which might turn out to 
    /// be strategically deficient.
    /// </summary>
    Interesting,
    /// <summary>
    /// <para>!: Good move</para>
    /// <para>While question marks indicate bad moves, exclamation points ("!") indicate good moves—especially ones 
    /// which are surprising or involve particular skill. Hence annotators are usually somewhat conservative 
    /// with the use of this symbol; for example, they would not annotate a game thus: 
    /// 1.e4! c5! 2.Nf3! d6! 3.d4! cxd4! 4.Nxd4! Nf6! 5.Nc3! All the moves of this main-line 
    /// Sicilian Defence are good ones, but the players have demonstrated little skill by simply following 
    /// well-known opening theory.</para>
    /// <para>Once the players start making good choices when faced with difficult decisions, however, a few moves 
    /// may receive exclamation points from annotators. Typical moves receiving exclamation points are strong opening 
    /// novelties, well-timed breakthroughs, sound sacrifices, and moves that avoid falling into traps.</para>
    /// </summary>
    Good,
    /// <summary>
    /// <para>‼: Brilliant move</para>
    /// The double exclamation point ("‼") is used to praise a move which the annotator thinks really shows 
    /// the player's skill. Such moves are usually hard to find. These may include sound sacrifices of large 
    /// amounts of material and moves that at first glance seem very counter-intuitive.
    /// </summary>
    Brilliant 
  }
}