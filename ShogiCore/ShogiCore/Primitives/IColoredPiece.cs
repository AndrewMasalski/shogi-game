namespace Yasc.ShogiCore.Primitives
{
  /// <summary>Represents a piece with color</summary>
  public interface IColoredPiece
  {
    /// <summary>Piece type</summary>
    IPieceType PieceType { get; }
    /// <summary>Piece color</summary>
    PieceColor Color { get; }

    /// <summary>Gets a promoted version of this piece</summary>
    IColoredPiece Promoted { get; }
  }
}