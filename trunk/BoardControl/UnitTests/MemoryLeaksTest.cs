using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.BoardControl.Controls;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.PieceSets;

namespace BoardControl.UnitTests
{
  [TestClass]
  public class MemoryLeaksTest
  {
    [TestMethod]
    public void CheckMemoryLeaksForShogiHand()
    {
      var board = new Board(new StandardPieceSet());
      var hand = new ShogiHand {Hand = board.White.Hand};

      var handWeakReference = new WeakReference(hand);
      hand = null;
      GC.Collect();
      Assert.IsNull(handWeakReference.Target);
    }
  }
}