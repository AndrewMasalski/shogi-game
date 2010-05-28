using System;
using Yasc.ShogiCore.Primitives;
using Yasc.ShogiCore.Snapshots;

namespace Yasc.ShogiCore.Moves
{
  /// <summary>Base class for the shogi move (usual and drop)</summary>
  public class DecoratedMove
  {
    /// <summary>Moment move is performed</summary>
    public DateTime Timestamp { get; private set; }
    /// <summary>Gets the color of player who made the move</summary>
    public PieceColor Who { get { return Move.Who; } }
    /// <summary>Move sequential number within the game</summary>
    public int Number { get; private set; }
    /// <summary>Gets the move data</summary>
    public Move Move { get; set; }
    /// <summary>Board snapshot before the move is done</summary>
    public BoardSnapshot BoardSnapshot
    {
      get { return Move.BoardSnapshot; }
    }

    /// <summary>ctor</summary>
    internal DecoratedMove(Move move, int number)
    {
      Timestamp = DateTime.Now;
      Number = number;
      Move = move;
    }

    /// <summary>Gets the string representation of the move</summary>
    public override string ToString()
    {
      return Move.ToString();
    }
  }
}