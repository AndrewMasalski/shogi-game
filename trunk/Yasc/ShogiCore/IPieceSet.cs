namespace Yasc.ShogiCore
{
  public interface IPieceSet
  {
    Piece this[PieceType type] { get; }
    void Take(Piece piece);
    void Return(Piece piece);
  }
}