using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore.Primitives;

namespace ShogiCore.UnitTests.ShogiCore
{
  [TestClass]
  public class PieceTypeTest
  {
    [TestMethod]
    public void IsPromotedPropertyTest()
    {
      Assert.IsFalse(PT.王.IsPromoted);
      Assert.IsFalse(PT.玉.IsPromoted);
      Assert.IsFalse(PT.飛.IsPromoted);
      Assert.IsFalse(PT.角.IsPromoted);
      Assert.IsFalse(PT.金.IsPromoted);
      Assert.IsFalse(PT.銀.IsPromoted);
      Assert.IsFalse(PT.桂.IsPromoted);
      Assert.IsFalse(PT.香.IsPromoted);
      Assert.IsFalse(PT.歩.IsPromoted);
      Assert.IsTrue(PT.竜.IsPromoted);
      Assert.IsTrue(PT.馬.IsPromoted);
      Assert.IsTrue(PT.全.IsPromoted);
      Assert.IsTrue(PT.今.IsPromoted);
      Assert.IsTrue(PT.仝.IsPromoted);
      Assert.IsTrue(PT.と.IsPromoted);
    }
    [TestMethod]
    public void CanPromotePropertyTest()
    {
      Assert.IsFalse(PT.王.CanPromote);
      Assert.IsFalse(PT.玉.CanPromote);
      Assert.IsTrue(PT.飛.CanPromote);
      Assert.IsTrue(PT.角.CanPromote);
      Assert.IsFalse(PT.金.CanPromote);
      Assert.IsTrue(PT.銀.CanPromote);
      Assert.IsTrue(PT.桂.CanPromote);
      Assert.IsTrue(PT.香.CanPromote);
      Assert.IsTrue(PT.歩.CanPromote);
      Assert.IsFalse(PT.竜.CanPromote);
      Assert.IsFalse(PT.馬.CanPromote);
      Assert.IsFalse(PT.全.CanPromote);
      Assert.IsFalse(PT.今.CanPromote);
      Assert.IsFalse(PT.仝.CanPromote);
      Assert.IsFalse(PT.と.CanPromote);
    } 
    [TestMethod]
    public void LatinPropertyTest()
    {
      Assert.AreEqual("Kr", PT.王.Latin);
      Assert.AreEqual("Kc", PT.玉.Latin);
      Assert.AreEqual("R", PT.飛.Latin);
      Assert.AreEqual("B", PT.角.Latin);
      Assert.AreEqual("G", PT.金.Latin);
      Assert.AreEqual("S", PT.銀.Latin);
      Assert.AreEqual("N", PT.桂.Latin);
      Assert.AreEqual("L", PT.香.Latin);
      Assert.AreEqual("P", PT.歩.Latin);
      Assert.AreEqual("Rp", PT.竜.Latin);
      Assert.AreEqual("Bp", PT.馬.Latin);
      Assert.AreEqual("Sp", PT.全.Latin);
      Assert.AreEqual("Np", PT.今.Latin);
      Assert.AreEqual("Lp", PT.仝.Latin);
      Assert.AreEqual("Pp", PT.と.Latin);
    } 
    [TestMethod]
    public void UnpromoteMethodTest()
    {
      Assert.AreEqual("R", PT.竜.Demote().Latin);
      Assert.AreEqual("B", PT.馬.Demote().Latin);
      Assert.AreEqual("S", PT.全.Demote().Latin);
      Assert.AreEqual("N", PT.今.Demote().Latin);
      Assert.AreEqual("L", PT.仝.Demote().Latin);
      Assert.AreEqual("P", PT.と.Demote().Latin);

      foreach (var p in new[] 
                            { 
                              PT.王, PT.玉, 
                              PT.金, PT.飛, 
                              PT.角, PT.銀, 
                              PT.桂, PT.香, PT.歩 
                            })
      {
        try
        {
          p.Demote();
          Assert.Fail("Can't unpromote " + p);
        }
        catch (InvalidOperationException x)
        {
          Assert.AreEqual(typeof(InvalidOperationException), x.GetType(),
            "We expected exception of type InvalidOperationException but " + 
            x.GetType().FullName + " is what we've got.");
        }
      }
    } 
    [TestMethod]
    public void PromoteMethodTest()
    {
      Assert.AreEqual("Rp", PT.飛.Promote().Latin);
      Assert.AreEqual("Bp", PT.角.Promote().Latin);
      Assert.AreEqual("Sp", PT.銀.Promote().Latin);
      Assert.AreEqual("Np", PT.桂.Promote().Latin);
      Assert.AreEqual("Lp", PT.香.Promote().Latin);
      Assert.AreEqual("Pp", PT.歩.Promote().Latin);

      foreach (var p in new[] { PT.王, PT.玉, PT.金, PT.竜, PT.馬, PT.全, PT.今, PT.仝, PT.と })
      {
        try
        {
          p.Promote();
          Assert.Fail("Can't promote " + p);
        }
        catch (InvalidOperationException x)
        {
          Assert.AreEqual(typeof(InvalidOperationException), x.GetType(),
            "We expected exception of type InvalidOperationException but " + 
            x.GetType().FullName + " is what we've got.");
        }
      }
    }
    [TestMethod]
    public void ToStringMethodTest()
    {
      Assert.AreEqual("王", PT.王.ToString());
      Assert.AreEqual("玉", PT.玉.ToString());
      Assert.AreEqual("飛", PT.飛.ToString());
      Assert.AreEqual("角", PT.角.ToString());
      Assert.AreEqual("金", PT.金.ToString());
      Assert.AreEqual("銀", PT.銀.ToString());
      Assert.AreEqual("桂", PT.桂.ToString());
      Assert.AreEqual("香", PT.香.ToString());
      Assert.AreEqual("歩", PT.歩.ToString());
      Assert.AreEqual("竜", PT.竜.ToString());
      Assert.AreEqual("馬", PT.馬.ToString());
      Assert.AreEqual("全", PT.全.ToString());
      Assert.AreEqual("今", PT.今.ToString());
      Assert.AreEqual("仝", PT.仝.ToString());
      Assert.AreEqual("と", PT.と.ToString());
    }
    [TestMethod]
    public void ToStringOperatorTest()
    {
      Assert.AreEqual("王", PT.王.ToString());
      Assert.AreEqual("玉", PT.玉.ToString());
      Assert.AreEqual("飛", PT.飛.ToString());
      Assert.AreEqual("角", PT.角.ToString());
      Assert.AreEqual("金", PT.金.ToString());
      Assert.AreEqual("銀", PT.銀.ToString());
      Assert.AreEqual("桂", PT.桂.ToString());
      Assert.AreEqual("香", PT.香.ToString());
      Assert.AreEqual("歩", PT.歩.ToString());
      Assert.AreEqual("竜", PT.竜.ToString());
      Assert.AreEqual("馬", PT.馬.ToString());
      Assert.AreEqual("全", PT.全.ToString());
      Assert.AreEqual("今", PT.今.ToString());
      Assert.AreEqual("仝", PT.仝.ToString());
      Assert.AreEqual("と", PT.と.ToString());
    }
    [TestMethod]
    public void FromStringOperatorTest()
    {
      Assert.AreEqual("Kr", (PT.王).Latin);
      Assert.AreEqual("Kc", (PT.玉).Latin);
      Assert.AreEqual("R", (PT.飛).Latin);
      Assert.AreEqual("B", (PT.角).Latin);
      Assert.AreEqual("G", (PT.金).Latin);
      Assert.AreEqual("S", (PT.銀).Latin);
      Assert.AreEqual("N", (PT.桂).Latin);
      Assert.AreEqual("L", (PT.香).Latin);
      Assert.AreEqual("P", (PT.歩).Latin);
      Assert.AreEqual("Rp", (PT.竜).Latin);
      Assert.AreEqual("Bp", (PT.馬).Latin);
      Assert.AreEqual("Sp", (PT.全).Latin);
      Assert.AreEqual("Np", (PT.今).Latin);
      Assert.AreEqual("Lp", (PT.仝).Latin);
      Assert.AreEqual("Pp", (PT.と).Latin);
    }
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void FromNullStringOperatorTest()
    {
      PT.Parse(null);
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void FromEmptyStringOperatorTest()
    {
      PT.Parse("");
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void FromIncorrectStringOperatorTest()
    {
      PT.Parse("d");
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void FromLongIncorrectStringOperatorTest()
    {
      PT.Parse("afsdfd");
    }
    [TestMethod]
    public void IdPropertyTest()
    {
      Assert.AreEqual(7, PT.王.PieceQuality.Id);
      Assert.AreEqual(8, PT.玉.PieceQuality.Id);

      Assert.AreEqual(7, PT.王.PieceKind.Id);
      Assert.AreEqual(7, PT.玉.PieceKind.Id);
      Assert.AreEqual(0, PT.飛.PieceKind.Id);
      Assert.AreEqual(1, PT.角.PieceKind.Id);
      Assert.AreEqual(2, PT.金.PieceKind.Id);
      Assert.AreEqual(3, PT.銀.PieceKind.Id);
      Assert.AreEqual(4, PT.桂.PieceKind.Id);
      Assert.AreEqual(5, PT.香.PieceKind.Id);
      Assert.AreEqual(6, PT.歩.PieceKind.Id);
      Assert.AreEqual(0, PT.竜.PieceKind.Id);
      Assert.AreEqual(1, PT.馬.PieceKind.Id);
      Assert.AreEqual(3, PT.全.PieceKind.Id);
      Assert.AreEqual(4, PT.今.PieceKind.Id);
      Assert.AreEqual(5, PT.仝.PieceKind.Id);
      Assert.AreEqual(6, PT.と.PieceKind.Id);
    }
    [TestMethod]
    public void GetIdsMethodTest()
    {
      CollectionAssert.AreEqual(new []{7, 0, 1, 2, 3, 4, 5, 6}, 
        PT.AllPieceKinds.Select(knd => knd.Id).ToList());
    }
    [TestMethod]
    public void GetValuesStaticMethodTest()
    {
      var expected = new[] { PT.王, PT.玉, PT.金, 
          PT.竜, PT.馬, PT.全, PT.今, PT.仝, PT.と,
          PT.飛, PT.角, PT.銀, PT.桂, PT.香, PT.歩
        };
      CollectionAssert.AreEquivalent(expected, PT.AllPieceTypes.ToList());
    }
    [TestMethod]
    public void GetPieceMethodTest()
    {
      var expected = new[] { PT.王, PT.金, PT.飛, 
        PT.角, PT.銀, PT.桂, PT.香, PT.歩 };
      var actual = from id in PT.AllPieceKinds select id.PieceTypes[0];
      CollectionAssert.AreEquivalent(expected, actual.ToList());
    }

    #region ' Equality '

    [TestMethod]
    public void ComparerTest()
    {
      Assert.IsTrue(PT.玉.CompareTo(PT.香) < 0);
      Assert.IsTrue(PT.香.CompareTo(PT.玉) > 0);
      Assert.AreEqual(0, PT.玉.CompareTo(PT.玉));
    }
    #endregion

  }
}
