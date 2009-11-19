using System;
using Yasc.Controls;
using Yasc.ShogiCore.Utils;

namespace Yasc.GenericDragDrop
{
  public class DropToBoardEventArgs : EventArgs
  {
    public DragFromEventArgs From { get; set; }
    public ShogiCell ToShogiCell { get; set; }
    public Cell ToCell
    {
      get { return (Cell)ToShogiCell.DataContext; }
    }
    public Position ToPosition
    {
      get { return ToCell.Position; }
    }

    public DropToBoardEventArgs(DragFromEventArgs from, ShogiCell cell)
    {
      From = from;
      ToShogiCell = cell;
    }
  }
}