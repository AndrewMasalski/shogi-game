using System;
using System.Collections.Generic;
using System.Linq;
using Yasc.BoardControl.Controls;
using Yasc.ShogiCore.Primitives;

namespace Yasc.RulesVisualization
{
  public abstract class DropMovesBase : MovesBase, IDropMoves
  {
    public PieceColor For { get; set; }
    public string Piece { get; set; }

    IPieceType IDropMoves.Piece
    {
      get { return PT.Parse(Piece); }
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
      return from s in To.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries) 
             select Position.Parse(s);
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

    public override void ShowMoves(ShogiBoard board)
    {
      var dropMoves = (IDropMoves)this;

      board.GetHand(dropMoves.For).GetPiece(dropMoves.Piece).IsMoveSource = true;

      foreach (var p in dropMoves.To)
        board.GetCell(p).IsPossibleMoveTarget = true;
    }
  }
}