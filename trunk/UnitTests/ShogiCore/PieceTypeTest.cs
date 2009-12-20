using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;

namespace UnitTests.ShogiCore
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
      Assert.AreEqual("R", PieceType.竜.Unpromote().Latin);
      Assert.AreEqual("B", PieceType.馬.Unpromote().Latin);
      Assert.AreEqual("S", PieceType.全.Unpromote().Latin);
      Assert.AreEqual("N", PieceType.今.Unpromote().Latin);
      Assert.AreEqual("L", PieceType.仝.Unpromote().Latin);
      Assert.AreEqual("P", PieceType.と.Unpromote().Latin);

      foreach (var p in new PieceType[] { "王", "玉", "金", "飛", "角", "銀", "桂", "香", "歩", })
      {
        try
        {
          p.Unpromote();
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

      foreach (var p in new PieceType[] { "王", "玉", "金", "竜", "馬", "全", "今", "仝", "と", })
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
      Assert.AreEqual("王", (string)PieceType.王);
      Assert.AreEqual("玉", (string)PieceType.玉);
      Assert.AreEqual("飛", (string)PieceType.飛);
      Assert.AreEqual("角", (string)PieceType.角);
      Assert.AreEqual("金", (string)PieceType.金);
      Assert.AreEqual("銀", (string)PieceType.銀);
      Assert.AreEqual("桂", (string)PieceType.桂);
      Assert.AreEqual("香", (string)PieceType.香);
      Assert.AreEqual("歩", (string)PieceType.歩);
      Assert.AreEqual("竜", (string)PieceType.竜);
      Assert.AreEqual("馬", (string)PieceType.馬);
      Assert.AreEqual("全", (string)PieceType.全);
      Assert.AreEqual("今", (string)PieceType.今);
      Assert.AreEqual("仝", (string)PieceType.仝);
      Assert.AreEqual("と", (string)PieceType.と);
    }
    [TestMethod]
    public void FromStringOperatorTest()
    {
      Assert.AreEqual("Kr", ((PieceType)"王").Latin);
      Assert.AreEqual("Kc", ((PieceType)"玉").Latin);
      Assert.AreEqual("R", ((PieceType)"飛").Latin);
      Assert.AreEqual("B", ((PieceType)"角").Latin);
      Assert.AreEqual("G", ((PieceType)"金").Latin);
      Assert.AreEqual("S", ((PieceType)"銀").Latin);
      Assert.AreEqual("N", ((PieceType)"桂").Latin);
      Assert.AreEqual("L", ((PieceType)"香").Latin);
      Assert.AreEqual("P", ((PieceType)"歩").Latin);
      Assert.AreEqual("Rp", ((PieceType)"竜").Latin);
      Assert.AreEqual("Bp", ((PieceType)"馬").Latin);
      Assert.AreEqual("Sp", ((PieceType)"全").Latin);
      Assert.AreEqual("Np", ((PieceType)"今").Latin);
      Assert.AreEqual("Lp", ((PieceType)"仝").Latin);
      Assert.AreEqual("Pp", ((PieceType)"と").Latin);
    }
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void FromNullStringOperatorTest()
    {
      ((PieceType) null).ToString();
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void FromEmptyStringOperatorTest()
    {
      ((PieceType) "").ToString();
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void FromIncorrectStringOperatorTest()
    {
      ((PieceType) "d").ToString();
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void FromLongIncorrectStringOperatorTest()
    {
      ((PieceType) "afsdfd").ToString();
    }
    [TestMethod]
    public void IdPropertyTest()
    {
      Assert.AreEqual(0, PieceType.王.Id);
      Assert.AreEqual(1, PieceType.玉.Id);
      Assert.AreEqual(2, PieceType.飛.Id);
      Assert.AreEqual(3, PieceType.角.Id);
      Assert.AreEqual(4, PieceType.金.Id);
      Assert.AreEqual(5, PieceType.銀.Id);
      Assert.AreEqual(6, PieceType.桂.Id);
      Assert.AreEqual(7, PieceType.香.Id);
      Assert.AreEqual(8, PieceType.歩.Id);
      Assert.AreEqual(2, PieceType.竜.Id);
      Assert.AreEqual(3, PieceType.馬.Id);
      Assert.AreEqual(5, PieceType.全.Id);
      Assert.AreEqual(6, PieceType.今.Id);
      Assert.AreEqual(7, PieceType.仝.Id);
      Assert.AreEqual(8, PieceType.と.Id);
    }
    [TestMethod]
    public void GetValuesStaticMethodTest()
    {
      var expected = new PieceType[] { "王", "玉", "金", 
          "竜", "馬", "全", "今", "仝", "と",
          "飛", "角", "銀", "桂", "香", "歩"
        };
      CollectionAssert.AreEquivalent(expected, PieceType.GetValues().ToList());
    }

    #region ' Equality '

    [TestMethod]
    public void TypedEqualsTest()
    {
      Assert.IsTrue(new PieceType().Equals(new PieceType()));
      Assert.IsTrue(PieceType.馬.Equals("馬"));
      Assert.IsFalse(((PieceType)"王").Equals("玉"));
    }
    [TestMethod]
    public void CommonEqualsTest()
    {
      Assert.IsFalse(new PieceType().Equals((object)null));
      Assert.IsFalse(new PieceType().Equals(new object()));
      Assert.IsFalse(new PieceType().Equals(6));
      Assert.IsTrue(new PieceType().Equals((object)new PieceType()));
    }
    [TestMethod]
    public void GetHashCodeTest()
    {
      var expected = from t in PieceType.GetValues() select t.GetHashCode();
      var actual = expected.Distinct();
      Assert.AreEqual(expected.Count(), actual.Count());
    }
    [TestMethod]
    public void OperatorsTest()
    {
      Assert.IsTrue(PieceType.香 == "香");
      Assert.IsTrue("香" != PieceType.玉);
    }
    [TestMethod]
    public void ComparerTest()
    {
      Assert.IsTrue(PieceType.玉.CompareTo("香") < 0);
      Assert.IsTrue(PieceType.香.CompareTo("玉") > 0);
      Assert.AreEqual(0, PieceType.玉.CompareTo("玉"));
    }
    #endregion

  }
}
