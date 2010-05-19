using System;
using Yasc.BoardControl.Controls;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.Primitives;

namespace Yasc.BoardControl.GenericDragDrop
{
  public class DropToBoardEventArgs : EventArgs
  {
    public DragFromEventArgs From { get; private set; }
    public ShogiCell ToShogiCell { get; private set; }
    public bool PromotionRequest { get; private set; }

    public Cell ToCell
    {
      get { return (Cell)ToShogiCell.DataContext; }
    }
    public Position ToPosition
    {
      get { return ToCell.Position; }
    }

    public DropToBoardEventArgs(DragFromEventArgs from, ShogiCell cell, bool promotionRequest)
    {
      From = from;
      ToShogiCell = cell;
      PromotionRequest = promotionRequest;
    }
  }
}