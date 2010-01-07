namespace Yasc.ShogiCore
{
  public interface IPieceSet
  {
    Piece this[PieceType type] { get; }
    void Pop(Piece piece);
    void Push(Piece piece);
  }
}