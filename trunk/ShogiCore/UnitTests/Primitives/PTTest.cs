using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore.Primitives;

namespace ShogiCore.UnitTests.Primitives
{
  [TestClass]
  public class PTTest
  {
    #region ' Parse '

    [TestMethod]
    public void ParseValidSymbolTest()
    {
      Assert.AreSame(PT.王, PT.Parse("王"));
      Assert.AreSame(PT.玉, PT.Parse("玉"));
      Assert.AreSame(PT.飛, PT.Parse("飛"));
      Assert.AreSame(PT.角, PT.Parse("角"));
      Assert.AreSame(PT.金, PT.Parse("金"));
      Assert.AreSame(PT.銀, PT.Parse("銀"));
      Assert.AreSame(PT.桂, PT.Parse("桂"));
      Assert.AreSame(PT.香, PT.Parse("香"));
      Assert.AreSame(PT.歩, PT.Parse("歩"));
      Assert.AreSame(PT.竜, PT.Parse("竜"));
      Assert.AreSame(PT.馬, PT.Parse("馬"));
      Assert.AreSame(PT.全, PT.Parse("全"));
      Assert.AreSame(PT.今, PT.Parse("今"));
      Assert.AreSame(PT.仝, PT.Parse("仝"));
      Assert.AreSame(PT.と, PT.Parse("と"));
    }
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void ParseNullTest()
    {
      PT.Parse(null);
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void ParseEmptyStringTest()
    {
      PT.Parse("");
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void ParseIncorrectStringTest()
    {
      PT.Parse("d");
    }
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void ParseLongIncorrectStringTest()
    {
      PT.Parse("afsdfd");
    }

    #endregion

    #region ' TryParse '

    [TestMethod]
    public void TryParseValidSymbolTest()
    {
      var vp = new Func<string, IPieceType>(s =>
      {
        IPieceType res;
        Assert.IsTrue(PT.TryParse(s, out res), "Couldn't parse valid symbol!");
        return res;
      });

      Assert.AreSame(PT.王, vp("王"));
      Assert.AreSame(PT.玉, vp("玉"));
      Assert.AreSame(PT.飛, vp("飛"));
      Assert.AreSame(PT.角, vp("角"));
      Assert.AreSame(PT.金, vp("金"));
      Assert.AreSame(PT.銀, vp("銀"));
      Assert.AreSame(PT.桂, vp("桂"));
      Assert.AreSame(PT.香, vp("香"));
      Assert.AreSame(PT.歩, vp("歩"));
      Assert.AreSame(PT.竜, vp("竜"));
      Assert.AreSame(PT.馬, vp("馬"));
      Assert.AreSame(PT.全, vp("全"));
      Assert.AreSame(PT.今, vp("今"));
      Assert.AreSame(PT.仝, vp("仝"));
      Assert.AreSame(PT.と, vp("と"));
    }
    [TestMethod]
    public void TryParseInvalidSymbolTest()
    {
      var checkSymbolCannotBeParsed = new Action<string>(s =>
      {
        IPieceType res;
        Assert.IsFalse(PT.TryParse(s, out res), "Couldn parse invalid symbol!");
      });
      checkSymbolCannotBeParsed("");
      checkSymbolCannotBeParsed("d");
      checkSymbolCannotBeParsed("dafsdf");
      checkSymbolCannotBeParsed("+9");
    }
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void TryParseNull()
    {
      IPieceType res;
      PT.TryParse(null, out res);
    }
    #endregion

    [TestMethod]
    public void AllPieceKindsTest()
    {
      CollectionAssert.AreEquivalent(
        new[] { PT.K, PT.R, PT.B, PT.G, PT.S, PT.N, PT.L, PT.P },
        PT.AllPieceKinds);
    }
    [TestMethod]
    public void AllPieceAllPieceQualitiesTest()
    {
      CollectionAssert.AreEquivalent(
        new[] { PT.Kr, PT.Kc, PT.R, PT.B, PT.G, PT.S, PT.N, PT.L, PT.P, },
        PT.AllPieceQualities);
    }
    [TestMethod]
    public void AllPieceTypesTest()
    {
      CollectionAssert.AreEquivalent(
        new[] { PT.王, PT.玉, PT.金, PT.竜, PT.馬, PT.全, PT.今, PT.仝, 
                PT.と, PT.飛, PT.角, PT.銀, PT.桂, PT.香, PT.歩 },
        PT.AllPieceTypes.ToList());
    }
  }
}