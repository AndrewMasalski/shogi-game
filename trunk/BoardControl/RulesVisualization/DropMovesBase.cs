using System;
using System.Collections.Generic;
using System.Linq;
using Yasc.ShogiCore;

namespace Yasc.RulesVisualization
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

    protected IEnumerable<Position> GetTo()
    {
      return from s in To.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries) select (Position)s;
    }
    protected IEnumerable<Position> GetNotTo()
    {
      switch (Mode)
      {
        case MovesValidatorMode.AndNoMore:
          var set = new HashSet<Position>(GetTo());
          return from p in Position.OnBoard where !set.Contains(p) select p;
        default:
          return new Position[0];
      }
    }
  }
}