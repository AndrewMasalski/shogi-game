using System;

namespace Yasc.ShogiCore.Snapshots
{
  /// <summary>Represents shogi game result</summary>
  [Flags]
  public enum ShogiGameState
  {
    /// <summary>It might be positional mate yet app didn't check it out</summary>
    NotDefined = 0x1,
    /// <summary>Game is going on</summary>
    None = 0x0, 
    /// <summary>Black won (either by mating white king, or white resign)</summary>
    BlackWin = 0x2,
    /// <summary>White won (either by mating black king, or black resign)</summary>
    WhiteWin = 0x4,
    /// <summary>Game ended with draw (either by repetition, impasse or an agreement)</summary>
    Draw = 0x8,
    DrawByRepitition = Draw | 0x10,
    CheckToWhite = 0x20, 
    CheckToBlack = 0x40
  }
}