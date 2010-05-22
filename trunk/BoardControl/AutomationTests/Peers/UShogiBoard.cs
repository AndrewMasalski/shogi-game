using System;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;
using Yasc.BoardControl.Controls;
using Yasc.ShogiCore.Primitives;

namespace BoardControl.AutomationTests.Peers
{
  public class UShogiBoard : WpfCustom
  {
    public UShogiBoard(UITestControl parent) 
      : base(parent)
    {
      SearchProperties[PropertyNames.ClassName] = typeof(ShogiBoard).UiaClassName();
    }

    public UShogiCell GetCell(Position position)
    {
      return new UShogiCell(this, position); 
    }
    public UShogiCell GetCell(string position)
    {
      return GetCell(Position.Parse(position));
    }
    public UShogiHand GetHand(PieceColor player)
    {
      return new UShogiHand(this, player); 
    }

    public void UsualMove(Position from, Position to)
    {
      var cell = GetCell(from);
      var piece = cell.Piece;
      Mouse.StartDragging(piece);
      Mouse.StopDragging(GetCell(to));
    }
    public void UsualMove(string positionFrom, string positionTo)
    {
      UsualMove(Position.Parse(positionFrom), Position.Parse(positionTo));
    }
    public void DropMove(IPieceType pieceType, PieceColor player, Position destination)
    {
      Mouse.StartDragging(GetHand(player)[pieceType].Piece);
      Mouse.StopDragging(GetCell(destination));
    }
    public void DropMove(IPieceType pieceType, PieceColor player, string destination)
    {
      DropMove(pieceType, player, Position.Parse(destination));
    }

    public UShogiPiece GetPiece(string position)
    {
      return GetCell(position).Piece;
    }
  }
}