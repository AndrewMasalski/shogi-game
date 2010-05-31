namespace Yasc.ShogiCore.Snapshots
{
  /// <summary>Represents shogi game result</summary>
  public enum ShogiGameState
  {
    /// <summary>It might be positional mate yet app didn't check it out</summary>
    NotDefined,
    /// <summary>Game is going on</summary>
    None, 
    /// <summary>Black won (either by mating white king, or white resign)</summary>
    BlackWin,
    /// <summary>White won (either by mating black king, or black resign)</summary>
    WhiteWin,
    /// <summary>Game ended with draw (either by repetition, impasse or an agreement)</summary>
    Draw,
    CheckToWhite, 
    CheckToBlack
  }
}