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
      Assert.IsFalse(PieceType.王.IsPromoted);
      Assert.IsFalse(PieceType.玉.IsPromoted);
      Assert.IsFalse(PieceType.飛.IsPromoted);
      Assert.IsFalse(PieceType.角.IsPromoted);
      Assert.IsFalse(PieceType.金.IsPromoted);
      Assert.IsFalse(PieceType.銀.IsPromoted);
      Assert.IsFalse(PieceType.桂.IsPromoted);
      Assert.IsFalse(PieceType.香.IsPromoted);
      Assert.IsFalse(PieceType.歩.IsPromoted);
      Assert.IsTrue(PieceType.竜.IsPromoted);
      Assert.IsTrue(PieceType.馬.IsPromoted);
      Assert.IsTrue(PieceType.全.IsPromoted);
      Assert.IsTrue(PieceType.今.IsPromoted);
      Assert.IsTrue(PieceType.仝.IsPromoted);
      Assert.IsTrue(PieceType.と.IsPromoted);
    }
    [TestMethod]
    public void CanPromotePropertyTest()
    {
      Assert.IsFalse(PieceType.王.CanPromote);
      Assert.IsFalse(PieceType.玉.CanPromote);
      Assert.IsTrue(PieceType.飛.CanPromote);
      Assert.IsTrue(PieceType.角.CanPromote);
      Assert.IsFalse(PieceType.金.CanPromote);
      Assert.IsTrue(PieceType.銀.CanPromote);
      Assert.IsTrue(PieceType.桂.CanPromote);
      Assert.IsTrue(PieceType.香.CanPromote);
      Assert.IsTrue(PieceType.歩.CanPromote);
      Assert.IsFalse(PieceType.竜.CanPromote);
      Assert.IsFalse(PieceType.馬.CanPromote);
      Assert.IsFalse(PieceType.全.CanPromote);
      Assert.IsFalse(PieceType.今.CanPromote);
      Assert.IsFalse(PieceType.仝.CanPromote);
      Assert.IsFalse(PieceType.と.CanPromote);
    } 
    [TestMethod]
    public void LatinPropertyTest()
    {
      Assert.AreEqual("Kr", PieceType.王.Latin);
      Assert.AreEqual("Kc", PieceType.玉.Latin);
      Assert.AreEqual("R", PieceType.飛.Latin);
      Assert.AreEqual("B", PieceType.角.Latin);
      Assert.AreEqual("G", PieceType.金.Latin);
      Assert.AreEqual("S", PieceType.銀.Latin);
      Assert.AreEqual("N", PieceType.桂.Latin);
      Assert.AreEqual("L", PieceType.香.Latin);
      Assert.AreEqual("P", PieceType.歩.Latin);
      Assert.AreEqual("Rp", PieceType.竜.Latin);
      Assert.AreEqual("Bp", PieceType.馬.Latin);
      Assert.AreEqual("Sp", PieceType.全.Latin);
      Assert.AreEqual("Np", PieceType.今.Latin);
      Assert.AreEqual("Lp", PieceType.仝.Latin);
      Assert.AreEqual("Pp", PieceType.と.Latin);
    } 
    [TestMethod]
    public void UnpromoteMethodTest()
    {
      Assert.AreEqual("R", PieceType.竜.Demote().Latin);
      Assert.AreEqual("B", PieceType.馬.Demote().Latin);
      Assert.AreEqual("S", PieceType.全.Demote().Latin);
      Assert.AreEqual("N", PieceType.今.Demote().Latin);
      Assert.AreEqual("L", PieceType.仝.Demote().Latin);
      Assert.AreEqual("P", PieceType.と.Demote().Latin);

      foreach (var p in new[] 
                            { 
                              PieceType.王, PieceType.玉, 
                              PieceType.金, PieceType.飛, 
                              PieceType.角, PieceType.銀, 
                              PieceType.桂, PieceType.香, PieceType.歩 
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
      Assert.AreEqual("Rp", PieceType.飛.Promote().Latin);
      Assert.AreEqual("Bp", PieceType.角.Promote().Latin);
      Assert.AreEqual("Sp", PieceType.銀.Promote().Latin);
      Assert.AreEqual("Np", PieceType.桂.Promote().Latin);
      Assert.AreEqual("Lp", PieceType.香.Promote().Latin);
      Assert.AreEqual("Pp", PieceType.歩.Promote().Latin);

      foreach (var p in new[] { PieceType.王, PieceType.玉, PieceType.金, PieceType.竜, PieceType.馬, PieceType.全, PieceType.今, PieceType.仝, PieceType.と })
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
      Assert.AreEqual("王", PieceType.王.ToString());
      Assert.AreEqual("玉", PieceType.玉.ToString());
      Assert.AreEqual("飛", PieceType.飛.ToString());
      Assert.AreEqual("角", PieceType.角.ToString());
      Assert.AreEqual("金", PieceType.金.ToString());
      Assert.AreEqual("銀", PieceType.銀.ToString());
      Assert.AreEqual("桂", PieceType.桂.ToString());
      Assert.AreEqual("香", PieceType.香.ToString());
      Assert.AreEqual("歩", PieceType.歩.ToString());
      Assert.AreEqual("竜", PieceType.竜.ToString());
      Assert.AreEqual("馬", PieceType.馬.ToString());
      Assert.AreEqual("全", PieceType.全.ToString());
      Assert.AreEqual("今", PieceType.今.ToString());
      Assert.AreEqual("仝", PieceType.仝.ToString());
      Assert.AreEqual("と", PieceType.と.ToString());
    }
    [TestMethod]
    public void ToStringOperatorTest()
    {
      Assert.AreEqual("王", PieceType.王.ToString());
      Assert.AreEqual("玉", PieceType.玉.ToString());
      Assert.AreEqual("飛", PieceType.飛.ToString());
      Assert.AreEqual("角", PieceType.角.ToString());
      Assert.AreEqual("金", PieceType.金.ToString());
      Assert.AreEqual("銀", PieceType.銀.ToString());
      Assert.AreEqual("桂", PieceType.桂.ToString());
      Assert.AreEqual("香", PieceType.香.ToString());
      Assert.AreEqual("歩", PieceType.歩.ToString());
      Assert.AreEqual("竜", PieceType.竜.ToString());
      Assert.AreEqual("馬", PieceType.馬.ToString());
      Assert.AreEqual("全", PieceType.全.ToString());
      Assert.AreEqual("今", PieceType.今.ToString());
      Assert.AreEqual("仝", PieceType.仝.ToString());
      Assert.AreEqual("と", PieceType.と.ToString());
    }
    [TestMethod]
    public void FromStringOperatorTest()
    {
      Assert.AreEqual("Kr", (PieceType.王).Latin);
      Assert.AreEqual("Kc", (PieceType.玉).Latin);
      Assert.AreEqual("R", (PieceType.飛).Latin);
      Assert.AreEqual("B", (PieceType.角).Latin);
      Assert.AreEqual("G", (PieceType.金).Latin);
      Assert.AreEqual("S", (PieceType.銀).Latin);
      Assert.AreEqual("N", (PieceType.桂).Latin);
      Assert.AreEqual("L", (PieceType.香).Latin);
      Assert.AreEqual("P", (PieceType.歩).Latin);
      Assert.AreEqual("Rp", (PieceType.竜).Latin);
      Assert.AreEqual("Bp", (PieceType.馬).Latin);
      Assert.AreEqual("Sp", (PieceType.全).Latin);
      Assert.AreEqual("Np", (PieceType.今).Latin);
      Assert.AreEqual("Lp", (PieceType.仝).Latin);
      Assert.AreEqual("Pp", (PieceType.と).Latin);
    }
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void FromNullStringOperatorTest()
    {
      PieceType.Parse(null);
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void FromEmptyStringOperatorTest()
    {
      PieceType.Parse("");
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void FromIncorrectStringOperatorTest()
    {
      PieceType.Parse("d");
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void FromLongIncorrectStringOperatorTest()
    {
      PieceType.Parse("afsdfd");
    }
    [TestMethod]
    public void IdPropertyTest()
    {
      Assert.AreEqual(7, PieceType.王.PieceQuality.Id);
      Assert.AreEqual(8, PieceType.玉.PieceQuality.Id);

      Assert.AreEqual(7, PieceType.王.PieceKind.Id);
      Assert.AreEqual(7, PieceType.玉.PieceKind.Id);
      Assert.AreEqual(0, PieceType.飛.PieceKind.Id);
      Assert.AreEqual(1, PieceType.角.PieceKind.Id);
      Assert.AreEqual(2, PieceType.金.PieceKind.Id);
      Assert.AreEqual(3, PieceType.銀.PieceKind.Id);
      Assert.AreEqual(4, PieceType.桂.PieceKind.Id);
      Assert.AreEqual(5, PieceType.香.PieceKind.Id);
      Assert.AreEqual(6, PieceType.歩.PieceKind.Id);
      Assert.AreEqual(0, PieceType.竜.PieceKind.Id);
      Assert.AreEqual(1, PieceType.馬.PieceKind.Id);
      Assert.AreEqual(3, PieceType.全.PieceKind.Id);
      Assert.AreEqual(4, PieceType.今.PieceKind.Id);
      Assert.AreEqual(5, PieceType.仝.PieceKind.Id);
      Assert.AreEqual(6, PieceType.と.PieceKind.Id);
    }
    [TestMethod]
    public void GetIdsMethodTest()
    {
      CollectionAssert.AreEqual(new []{7, 0, 1, 2, 3, 4, 5, 6}, 
        PieceType.AllPieceKinds.Select(knd => knd.Id).ToList());
    }
    [TestMethod]
    public void GetValuesStaticMethodTest()
    {
      var expected = new[] { PieceType.王, PieceType.玉, PieceType.金, 
          PieceType.竜, PieceType.馬, PieceType.全, PieceType.今, PieceType.仝, PieceType.と,
          PieceType.飛, PieceType.角, PieceType.銀, PieceType.桂, PieceType.香, PieceType.歩
        };
      CollectionAssert.AreEquivalent(expected, PieceType.AllPieceTypes.ToList());
    }
    [TestMethod]
    public void GetPieceMethodTest()
    {
      var expected = new[] { PieceType.王, PieceType.金, PieceType.飛, 
        PieceType.角, PieceType.銀, PieceType.桂, PieceType.香, PieceType.歩 };
      var actual = from id in PieceType.AllPieceKinds select id.PieceTypes[0];
      CollectionAssert.AreEquivalent(expected, actual.ToList());
    }

    #region ' Equality '

    [TestMethod]
    public void ComparerTest()
    {
      Assert.IsTrue(PieceType.玉.CompareTo(PieceType.香) < 0);
      Assert.IsTrue(PieceType.香.CompareTo(PieceType.玉) > 0);
      Assert.AreEqual(0, PieceType.玉.CompareTo(PieceType.玉));
    }
    #endregion

  }
}
