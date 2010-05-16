using System;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.BoardControl.Controls;
using Yasc.ShogiCore;

namespace BoardControl.UnitTests
{
  [TestClass]
  public class MemoryLeaksTest
  {
    [TestMethod]
    public void Test()
    {
      var board = new Board();
      var hand = new ShogiHand {Hand = board.White.Hand};

      var handWeakReference = new WeakReference(hand);
      hand = null;
      GC.Collect();
      Assert.IsNull(handWeakReference.Target);
    }
  }
}