using System;
using CommonUtils.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore.Primitives;

namespace ShogiCore.UnitTests.Primitives
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
      Assert.AreEqual("+R", PT.竜.Latin);
      Assert.AreEqual("+B", PT.馬.Latin);
      Assert.AreEqual("+S", PT.全.Latin);
      Assert.AreEqual("+N", PT.今.Latin);
      Assert.AreEqual("+L", PT.仝.Latin);
      Assert.AreEqual("+P", PT.と.Latin);
    } 
    [TestMethod]
    public void DemoteMethodTest()
    {
      Assert.AreSame(PT.飛, PT.竜.Demote());
      Assert.AreSame(PT.角, PT.馬.Demote());
      Assert.AreSame(PT.銀, PT.全.Demote());
      Assert.AreSame(PT.桂, PT.今.Demote());
      Assert.AreSame(PT.香, PT.仝.Demote());
      Assert.AreSame(PT.歩, PT.と.Demote());

      foreach (var p in new[] { PT.王, PT.玉, PT.金, PT.飛,  PT.角, PT.銀, PT.桂, PT.香, PT.歩 })
      {
        try
        {
          p.Demote();
          Assert.Fail("Could demote " + p);
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
      Assert.AreSame(PT.竜, PT.飛.Promote());
      Assert.AreSame(PT.馬, PT.角.Promote());
      Assert.AreSame(PT.全, PT.銀.Promote());
      Assert.AreSame(PT.今, PT.桂.Promote());
      Assert.AreSame(PT.仝, PT.香.Promote());
      Assert.AreSame(PT.と, PT.歩.Promote());

      foreach (var p in new[] { PT.王, PT.玉, PT.金, PT.竜, PT.馬, PT.全, PT.今, PT.仝, PT.と })
      {
        try
        {
          p.Promote();
          Assert.Fail("Could promote " + p);
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
    public void ToStringTest()
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
    public void JapaneseTest()
    {
      Assert.AreEqual("王", PT.王.Japanese);
      Assert.AreEqual("玉", PT.玉.Japanese);
      Assert.AreEqual("飛", PT.飛.Japanese);
      Assert.AreEqual("角", PT.角.Japanese);
      Assert.AreEqual("金", PT.金.Japanese);
      Assert.AreEqual("銀", PT.銀.Japanese);
      Assert.AreEqual("桂", PT.桂.Japanese);
      Assert.AreEqual("香", PT.香.Japanese);
      Assert.AreEqual("歩", PT.歩.Japanese);
      Assert.AreEqual("竜", PT.竜.Japanese);
      Assert.AreEqual("馬", PT.馬.Japanese);
      Assert.AreEqual("全", PT.全.Japanese);
      Assert.AreEqual("今", PT.今.Japanese);
      Assert.AreEqual("仝", PT.仝.Japanese);
      Assert.AreEqual("と", PT.と.Japanese);
    }
    [TestMethod]
    public void SerializableTest()
    {
      foreach (var pieceType in PT.AllPieceTypes)
       Assert.AreSame(pieceType, pieceType.SerializeDeserialize());
    }
  }
}
