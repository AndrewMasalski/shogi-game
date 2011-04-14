namespace Yasc.ShogiCore.Primitives
{
  /// <summary>Reperesents one of directions of moves piece can do </summary>
  public interface IMoveDirection
  {
    /// <summary>Delta X of the move</summary>
    int Dx { get; }
    /// <summary>Delta Y of the move</summary>
    int Dy { get; }
    /// <summary>Maximum count of moves in this direction </summary>
    int Count { get; }
  }
}