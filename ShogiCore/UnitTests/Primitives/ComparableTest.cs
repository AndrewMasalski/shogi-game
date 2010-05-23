using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore.Primitives;

namespace ShogiCore.UnitTests.Primitives
{
  [TestClass]
  public class ComparableTest
  {
    [TestMethod]
    public void PieceTypesTest()
    {
      Check(
        PT.王, PT.玉, PT.飛, PT.角, PT.金,
        PT.銀, PT.桂, PT.香, PT.歩, PT.竜,
        PT.馬, PT.全, PT.今, PT.仝, PT.と);
    }    
    [TestMethod]
    public void PieceCategoriesTest()
    {
      Check(PT.Kr, PT.Kc, PT.R, PT.B, PT.G, PT.S, PT.N, PT.L, PT.P);
      Check(PT.K, PT.R, PT.B, PT.G, PT.S, PT.N, PT.L, PT.P);
    }
    private static void Check<T>(params T[] orderedItems)
      where T: IComparable<T>
    {
      for (var i = 0; i < orderedItems.Length; i++)
        for (var j = 0; j < orderedItems.Length; j++)
          Assert.AreEqual(i.CompareTo(j), 
            Math.Sign(orderedItems[i].CompareTo(orderedItems[j])));
    }
  }
}