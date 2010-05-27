using Yasc.ShogiCore.Primitives;

namespace Yasc.ShogiCore.Snapshots
{
  /// <summary>Set of extension methods which makes work with 
  ///   grid handy althogh don't add any functionality</summary>
  public static class BoardSnapshotExtensions
  {
    /// <summary>Gets the piece snapshot at the <paramref name="position"/></summary>
    public static IColoredPiece GetPieceAt(this BoardSnapshot board, string position)
    {
      return board.GetPieceAt(Position.Parse(position));
    }
  }
}