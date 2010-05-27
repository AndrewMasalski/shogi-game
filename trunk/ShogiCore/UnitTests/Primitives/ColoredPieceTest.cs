using CommonUtils.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore.Primitives;

namespace ShogiCore.UnitTests.Primitives
{
  [TestClass]
  public class ColoredPieceTest
  {
    [TestMethod]
    public void PieceKindTest()
    {
      foreach (var pieceType in PT.AllPieceTypes)
      {
        Assert.AreSame(pieceType, pieceType.White.PieceType);
        Assert.AreSame(pieceType, pieceType.Black.PieceType);
        Assert.AreSame(pieceType.Black, pieceType.GetColored(PieceColor.Black));
        Assert.AreSame(pieceType.White, pieceType.GetColored(PieceColor.White));
      }
    }
    [TestMethod]
    public void SerializableTest()
    {
      foreach (var pieceType in PT.AllPieceTypes)
      {
        Assert.AreSame(pieceType.White, pieceType.White.SerializeDeserialize());
        Assert.AreSame(pieceType.Black, pieceType.Black.SerializeDeserialize());
      }
    }
  }
}