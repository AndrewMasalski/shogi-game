using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.Utils;
using System.Linq;

namespace CommonUtils.UnitTests
{
  [TestClass]
  public class ListUtilsUpdateWithConvertTest : ListUtilsUpdateTestBase
  {
    [TestMethod]
    public void TheSameTest()
    {
      Check(new[] { "1", "2", "3", "4", "5" }, new[] { 1, 2, 3, 4, 5 }, "");
      Check(new[] { "1", "2", "3", "4", "5" }, new[] { 1, 2, 99, 3, 4, 5 }, "Add(2, [99])");
      Check(new[] { "1", "2", "3", "4", "5" }, new[] { 1, 2, 99, 3, 98, 5 }, "Add(2, [99]) Replace(4, [98])");
      Check(new[] { "1", "2", "3", "4", "5" }, new[] { 1, 2 }, "Remove(4, [5]) Remove(3, [4]) Remove(2, [3])");
      Check(new[] { "1", "2" }, new[] { 1, 2, 3, 4, 5 }, "Add(2, [3]) Add(3, [4]) Add(4, [5])");
      Check(new[] { "1", "2", "3" }, new[] { 4, 3, 1 }, "Add(0, [4]) Move(3<->1, [3]) Remove(3, [2])");
    }
    [TestMethod]
    public void TestBig()
    {
      var list = new ObservableCollection<int>();
      for (int i = 0; i < 100; i++)
        list.Add(i);

      var set = new HashSet<int>();
      var r = new List<string>();
      var rnd = new Random(0);
      for (int i = 0; i < 100; i++)
      {
        int n = rnd.Next(150);
        if (set.Add(n))
          r.Add(n.ToString());
      }
      var log = new TestLog();
      list.CollectionChanged += (s, e) => log.Write(ToString(e));

      list.Update(r, i=>i.ToString(), s => s, s=>int.Parse(s));
      CollectionAssert.AreEqual(r, list.Select(i => i.ToString()).ToArray());
    }

    private static void Check(IEnumerable<string> oldList, int[] newList, string expectedLog)
    {
      var log = new TestLog();
      var observableColection = new ObservableCollection<string>(oldList);
      observableColection.CollectionChanged += (s, e) => log.Write(ToString(e));

      observableColection.Update(newList, s => s, i => i.ToString(), i => i.ToString());
      Assert.AreEqual(expectedLog, log.ToString());
      CollectionAssert.AreEqual(observableColection, newList.Select(i => i.ToString()).ToArray());
    }
  }
}