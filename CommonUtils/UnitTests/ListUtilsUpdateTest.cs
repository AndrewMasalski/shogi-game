using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.Utils;

namespace CommonUtils.UnitTests
{
  [TestClass]
  public class ListUtilsUpdateTest : ListUtilsUpdateTestBase
  {
    [TestMethod]
    public void TheSameTest()
    {
      Check(new[] { 1, 2, 3, 4, 5 }, new[] { 1, 2, 3, 4, 5 }, "");
      Check(new[] { 1, 2, 3, 4, 5 }, new[] { 1, 2, 99, 3, 4, 5 }, "Add(2, [99])");
      Check(new[] { 1, 2, 3, 4, 5 }, new[] { 1, 2, 99, 3, 98, 5 }, "Add(2, [99]) Replace(4, [98])");
      Check(new[] { 1, 2, 3, 4, 5 }, new[] { 1, 2 }, "Remove(4, [5]) Remove(3, [4]) Remove(2, [3])");
      Check(new[] { 1, 2 }, new[] { 1, 2, 3, 4, 5 }, "Add(2, [3]) Add(3, [4]) Add(4, [5])");
      Check(new[] { 1, 2, 3 }, new[] { 4, 3, 1 }, "Add(0, [4]) Move(3<->1, [3]) Remove(3, [2])");
    }
    [TestMethod]
    public void TestBig()
    {
      var list = new ObservableCollection<int>();
      for (int i = 0; i < 100; i++)
        list.Add(i);

      var set = new HashSet<int>();
      var r = new List<int>();
      var rnd = new Random(0);
      for (int i = 0; i < 100; i++)
      {
        int n = rnd.Next(150);
        if (set.Add(n))
          r.Add(n);
      }
      var log = new TestLog();
      list.CollectionChanged += (s, e) => log.Write(ToString(e));

      list.Update(r);
      CollectionAssert.AreEqual(r, list);
    }
    [TestMethod, Ignore]
    public void PerformanceTest()
    {
      var src = new List<int>();
      for (int i = 0; i < 10000; i++)
        src.Add(i);
      var spear = new List<int>();
      for (int i = 100000; i < 110000; i++)
        spear.Add(i);

      var sw = new Stopwatch();
      var rnd = new Random(0);
      var list = new ObservableCollection<int>(src);
      for (int i = 0; i < 100; i++)
      {
        MakeFewChanges(src, rnd, spear);
        sw.Start();
        list.Update(src);
        sw.Stop();
      }
      Console.WriteLine("Average update time: " + sw.ElapsedMilliseconds/100 + "ms");
    }

    private static void MakeFewChanges(IList<int> list, Random rnd, IList<int> spear)
    {
      for (int i = 0; i < 5; i++)
      {
        int index = rnd.Next(list.Count);
        switch (rnd.Next(5))
        {
          case 0: 
            spear.Add(list[index]);
            list.RemoveAt(index);
            break;
          case 1:
            {
              int spearIndex = rnd.Next(spear.Count);
              int spearElement = spear[spearIndex];
              spear.RemoveAt(spearIndex);
              list.Insert(index, spearElement);
            }
            break;
          case 2:
            {
              int spearIndex = rnd.Next(spear.Count);
              int spearElement = spear[spearIndex];
              spear.RemoveAt(spearIndex);
              list.Add(spearElement);
            }
            break;
          case 3:
            {
              int spearIndex = rnd.Next(spear.Count);
              int spearElement = spear[spearIndex];
              spear.RemoveAt(spearIndex);
              spear.Add(list[index]);
              list[index] = spearElement;
            }
            break;
          case 4:
            {
              var t = list[index];
              list.RemoveAt(index);
              list.Insert(rnd.Next(list.Count), t);
            }
            break;

        }
      }
    }
    private static void Check(IEnumerable<int> oldList, int[] newList, string expectedLog)
    {
      var log = new TestLog();
      var observableColection = new ObservableCollection<int>(oldList);
      observableColection.CollectionChanged += (s, e) => log.Write(ToString(e));

      observableColection.Update(newList);
      Assert.AreEqual(expectedLog, log.ToString());
      CollectionAssert.AreEqual(observableColection, newList);
    }
  }
}