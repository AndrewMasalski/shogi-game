using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.ShogiCore;

namespace UnitTests
{
  [TestClass]
  public class PlayerTest
  {
    [TestMethod]
    public void ResetAllPiecesFromHand()
    {
      var board = new Board();
      board.White.AddToHand("馬");
      board.White.ResetAllPiecesFromHand();
      Assert.AreEqual(0, board.White.Hand.Count);
    }
  }
}