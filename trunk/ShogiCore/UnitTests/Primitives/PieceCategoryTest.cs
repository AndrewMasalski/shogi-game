using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore.Primitives;

namespace ShogiCore.UnitTests.Primitives
{
  [TestClass]
  public class PieceCategoryTest
  {
    [TestMethod]
    public void PieceKindTest()
    {
      Assert.AreSame(PT.K, PT.王.PieceKind);
      Assert.AreSame(PT.K, PT.玉.PieceKind);
      Assert.AreSame(PT.R, PT.飛.PieceKind);
      Assert.AreSame(PT.B, PT.角.PieceKind);
      Assert.AreSame(PT.G, PT.金.PieceKind);
      Assert.AreSame(PT.S, PT.銀.PieceKind);
      Assert.AreSame(PT.N, PT.桂.PieceKind);
      Assert.AreSame(PT.L, PT.香.PieceKind);
      Assert.AreSame(PT.P, PT.歩.PieceKind);
      Assert.AreSame(PT.R, PT.竜.PieceKind);
      Assert.AreSame(PT.B, PT.馬.PieceKind);
      Assert.AreSame(PT.S, PT.全.PieceKind);
      Assert.AreSame(PT.N, PT.今.PieceKind);
      Assert.AreSame(PT.L, PT.仝.PieceKind);
      Assert.AreSame(PT.P, PT.と.PieceKind);
    }
    [TestMethod]
    public void PieceQualityTest()
    {
      Assert.AreSame(PT.Kr, PT.王.PieceQuality);
      Assert.AreSame(PT.Kc, PT.玉.PieceQuality);
      Assert.AreSame(PT.R, PT.飛.PieceQuality);
      Assert.AreSame(PT.B, PT.角.PieceQuality);
      Assert.AreSame(PT.G, PT.金.PieceQuality);
      Assert.AreSame(PT.S, PT.銀.PieceQuality);
      Assert.AreSame(PT.N, PT.桂.PieceQuality);
      Assert.AreSame(PT.L, PT.香.PieceQuality);
      Assert.AreSame(PT.P, PT.歩.PieceQuality);
      Assert.AreSame(PT.R, PT.竜.PieceQuality);
      Assert.AreSame(PT.B, PT.馬.PieceQuality);
      Assert.AreSame(PT.S, PT.全.PieceQuality);
      Assert.AreSame(PT.N, PT.今.PieceQuality);
      Assert.AreSame(PT.L, PT.仝.PieceQuality);
      Assert.AreSame(PT.P, PT.と.PieceQuality);
    }
    [TestMethod]
    public void IdTest()
    {
      Assert.AreEqual(0, PT.R.Id);
      Assert.AreEqual(1, PT.B.Id);
      Assert.AreEqual(2, PT.G.Id);
      Assert.AreEqual(3, PT.S.Id);
      Assert.AreEqual(4, PT.N.Id);
      Assert.AreEqual(5, PT.L.Id);
      Assert.AreEqual(6, PT.P.Id);
      Assert.AreEqual(7, PT.K.Id);
      Assert.AreEqual(7, PT.Kr.Id);
      Assert.AreEqual(8, PT.Kc.Id);
    }
    [TestMethod]
    public void PieceTypesTest()
    {
      CollectionAssert.AreEqual(new[] { PT.王, PT.玉 }, PT.K.PieceTypes);
      CollectionAssert.AreEqual(new[] { PT.王 }, PT.Kr.PieceTypes);
      CollectionAssert.AreEqual(new[] { PT.玉 }, PT.Kc.PieceTypes);
      CollectionAssert.AreEqual(new[] { PT.飛, PT.竜 }, PT.R.PieceTypes);
      CollectionAssert.AreEqual(new[] { PT.角, PT.馬 }, PT.B.PieceTypes);
      CollectionAssert.AreEqual(new[] { PT.金 }, PT.G.PieceTypes);
      CollectionAssert.AreEqual(new[] { PT.銀, PT.全 }, PT.S.PieceTypes);
      CollectionAssert.AreEqual(new[] { PT.桂, PT.今 }, PT.N.PieceTypes);
      CollectionAssert.AreEqual(new[] { PT.香, PT.仝 }, PT.L.PieceTypes);
      CollectionAssert.AreEqual(new[] { PT.歩, PT.と }, PT.P.PieceTypes);
    }
  }
}