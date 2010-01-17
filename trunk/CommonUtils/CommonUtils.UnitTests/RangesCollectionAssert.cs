using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.Utils;

namespace CommonUtils.UnitTests
{
  public static class RangesCollectionAssert
  {
    public static void AreSame<T>(IEnumerable<Range<T>> expected, IEnumerable<Range<T>> actual)
    {
      int counter = 0;
      var ae = expected.GetEnumerator();
      var be = actual.GetEnumerator();
      while (true)
      {
        bool an = ae.MoveNext();
        bool bn = be.MoveNext();
        Assert.AreEqual(an, bn, "Collections have different length");
        if (!an) return;
        Assert.IsTrue(ae.Current.SameAs(be.Current), "Elements at index " + counter + " are different");
        counter++;
      }
    }
  }
}