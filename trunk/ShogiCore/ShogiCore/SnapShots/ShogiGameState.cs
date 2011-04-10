using System;

namespace Yasc.ShogiCore.Snapshots
{
  /// <summary>Contains shogi game flags such as 
  ///   "game is over" or if "it's check now"</summary>
  [Flags]
  public enum ShogiGameState
  {
    /// <summary>No flags set: game is not over yet</summary>
    None = 0x0,
    /// <summary>It might be positional mate yet app didn't check it out</summary>
    NotDefined = 0x1,
    /// <summary>Black won (either by mating white king, or white resign)</summary>
    BlackWin = 0x2,
    /// <summary>White won (either by mating black king, or black resign)</summary>
    WhiteWin = 0x4,
    /// <summary>Game ended with draw (either by repetition, impasse or an agreement)</summary>
    Draw = 0x8,
    /// <summary>Game ended with draw by repetition</summary>
    DrawByRepitition = Draw | 0x10,
    /// <summary>It's check to white</summary>
    CheckToWhite = 0x20,
    /// <summary>It's check to black</summary>
    CheckToBlack = 0x40
  }
}