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
      var hand = new ShogiHand();
      var collection = new ObservableCollection<Piece>();
      hand.Hand = collection;

      var handWeakReference = new WeakReference(hand);
      hand = null;
      GC.Collect();
      Assert.IsNull(handWeakReference.Target);
    }
  }
}