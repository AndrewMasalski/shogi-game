using System;
using Yasc.Controls;

namespace Yasc.GenericDragDrop
{
  public class DropToHandEventArgs : EventArgs
  {
    public DragFromEventArgs From { get; private set; }
    public ShogiHand ToHand { get; private set; }

    public DropToHandEventArgs(DragFromEventArgs from, ShogiHand hand)
    {
      From = from;
      ToHand = hand;
    }
  }
}