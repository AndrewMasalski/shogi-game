using Yasc.ShogiCore.Primitives;

namespace Yasc.ShogiCore.Core
{
  /// <summary>Represents piece set</summary>
  public interface IPieceSet
  {
    /// <summary>Gets reference to the piece from the set by type</summary>
    Piece this[IPieceType type] { get; }
    /// <summary>Marks given <paramref name="piece"/> as occupied</summary>
    void AcquirePiece(Piece piece);
    /// <summary>Returns given <paramref name="piece"/> to the set</summary>
    void ReleasePiece(Piece piece);
  }
}