namespace Yasc.ShogiCore.Core
{
  /// <summary>Represents shogi game result</summary>
  public enum ShogiGameResult
  {
    /// <summary>Game is not finished yet</summary>
    None, 
    /// <summary>Black won</summary>
    BlackWin,
    /// <summary>White won</summary>
    WhiteWin,
    /// <summary>Game ended with draw</summary>
    Draw
  }
}