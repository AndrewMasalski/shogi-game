using Yasc.ShogiCore.Primitives;

namespace Yasc.ShogiCore.Core
{
  /// <summary>Represents piece set</summary>
  public interface IPieceSet
  {
    /// <summary>Gets reference to the piece from the set by type</summary>
    Piece this[PieceType type] { get; }
    /// <summary>Marks given <paramref name="piece"/> as occupied</summary>
    void Pop(Piece piece);
    /// <summary>Returns given <paramref name="piece"/> to the set</summary>
    void Push(Piece piece);
  }
}