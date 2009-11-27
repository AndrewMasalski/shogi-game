using System.Collections.Generic;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Utils;

namespace RulesVisualization
{
  public abstract class DropMovesBase : MovesBase, IDropMoves
  {
    public PieceColor For { get; set; }
    public string Piece { get; set; }

    PieceType IDropMoves.Piece
    {
      get { return Piece; }
    }
    IEnumerable<Position> IDropMoves.To
    {
      get { return IsAvailable ? GetTo() : GetNotTo(); }
    }
    IEnumerable<Position> IDropMoves.NotTo
    {
      get { return IsAvailable ? GetNotTo() : GetTo(); }
    }
  }
}