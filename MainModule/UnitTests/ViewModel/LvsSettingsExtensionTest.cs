using System;
using System.Collections.Generic;
using System.Linq;
using MainModule.Properties;
using MainModule.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MainModule.UnitTests.ViewModel
{
  [TestClass]
  public class LvsSettingsExtensionTest
  {
    private static readonly Random _rnd = new Random();

    [TestInitialize]
    public void Init()
    {
      Reset(Settings.Default);
    }

    [TestMethod]
    public void LoadEmptyListTest()
    {
      var actual = Enumerable.ToList<string>(Settings.Default.LoadLvs());
      var expected = new string[0];
      CollectionAssert.AreEqual(expected, actual);
    }
    [TestMethod]
    public void LoadAndSaveOneItemTest()
    {
      var expected = new[] { "item1" };
      Settings.Default.SaveLvs(expected);

      Assert.AreEqual("item1", Settings.Default.LastVisitedServer1);
      Assert.AreEqual(null, Settings.Default.LastVisitedServer2);
      Assert.AreEqual(null, Settings.Default.LastVisitedServer3);
      Assert.AreEqual(null, Settings.Default.LastVisitedServer4);
      Assert.AreEqual(null, Settings.Default.LastVisitedServer5);
      Assert.AreEqual(null, Settings.Default.LastVisitedServer6);
      Assert.AreEqual(null, Settings.Default.LastVisitedServer7);
      Assert.AreEqual(null, Settings.Default.LastVisitedServer8);
      Assert.AreEqual(null, Settings.Default.LastVisitedServer9);
      Assert.AreEqual(null, Settings.Default.LastVisitedServer10);

      var actual = Settings.Default.LoadLvs().ToList();

      CollectionAssert.AreEqual(expected, actual);
    }
    [TestMethod]
    public void LoadAndSaveDifferentItemsCount()
    {
      for (int i = 10; i >= 0; i--)
        LoadAndSaveItems(i);
    }
    [TestMethod]
    public void LoadAndSaveMoreThan10()
    {
      var expected = CreateList(12);
      Settings.Default.SaveLvs(expected);
      expected.RemoveAt(11);
      expected.RemoveAt(10);

      var actual = Settings.Default.LoadLvs().ToList();
      CollectionAssert.AreEqual(expected, actual);
    }

    public void LoadAndSaveItems(int count)
    {
      var expected = CreateList(count);
      Settings.Default.SaveLvs(expected);
      var actual = Settings.Default.LoadLvs().ToList();
      CollectionAssert.AreEqual(expected, actual);
    }
    private static List<string> CreateList(int count)
    {
      var expected = new List<string>();
      for (int i = 0; i < count; i++)
        expected.Add("item" + (i + 1) + _rnd.Next(1000));
      return expected;
    }
    private static void Reset(Settings settings)
    {
      settings.LastVisitedServer1 = null;
      settings.LastVisitedServer2 = null;
      settings.LastVisitedServer3 = null;
      settings.LastVisitedServer4 = null;
      settings.LastVisitedServer5 = null;
      settings.LastVisitedServer6 = null;
      settings.LastVisitedServer7 = null;
      settings.LastVisitedServer8 = null;
      settings.LastVisitedServer9 = null;
      settings.LastVisitedServer10 = null;
    }
  }
}