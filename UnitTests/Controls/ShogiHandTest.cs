using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.Controls;
using Yasc.ShogiCore;

namespace UnitTests.Controls
{
  [TestClass]
  public class ShogiHandTest
  {
    [TestMethod]
    public void ConstructionTest()
    {
      var board = new Board();
      var hand = new ShogiHand {Hand = board.White.Hand};
      Assert.IsNull(hand.Board);
      Assert.IsFalse(hand.IsGrouping);
      Assert.AreEqual(0, hand.Items.Count);
    }
    [TestMethod]
    public void FlatTest()
    {
      var board = new Board();
      var hand = new ShogiHand { Color = PieceColor.Black, Board = board};
      board.Black.AddToHand(PieceType.馬);
      Assert.AreEqual(PieceType.角, hand.Items[0].PieceType);
      Assert.AreEqual(PieceColor.Black, hand.Items[0].PieceColor);

      hand.Color = PieceColor.White;
      Assert.AreEqual(0, hand.Items.Count);
      Assert.AreSame(board.White.Hand, hand.Hand);
      board.White.AddToHand(PieceType.歩);
      board.White.AddToHand(PieceType.歩);
      board.White.AddToHand(PieceType.歩);
      board.White.AddToHand(PieceType.歩);
      Assert.AreEqual(4, hand.Items.Count);
      board.White.Hand.RemoveAt(3);
      Assert.AreEqual(3, hand.Items.Count);
    }
    [TestMethod]
    public void FlatTestNestDetails()
    {
      var board = new Board();
      var hand = new ShogiHand { Color = PieceColor.Black, Board = board};
      board.Black.AddToHand(PieceType.歩);
      board.Black.AddToHand(PieceType.歩);
      board.Black.AddToHand(PieceType.歩);
      board.Black.AddToHand(PieceType.歩);
      var handNest = hand.Items[2];
      Assert.AreEqual(PieceColor.Black, handNest.PieceColor);
      Assert.AreEqual(1, handNest.PiecesCount);
    }
    [TestMethod]
    public void GroupTest()
    {
      var board = new Board();
      var hand = new ShogiHand
                   {
                     Color = PieceColor.Black,
                     IsGrouping = true, 
                     Board = board
                   };
      board.Black.AddToHand(PieceType.歩);
      board.Black.AddToHand(PieceType.歩);
      board.Black.AddToHand(PieceType.歩);
      board.Black.AddToHand(PieceType.歩);
      Assert.AreEqual(1, hand.Items.Count);
      var handNest = hand.Items[0];
      Assert.AreEqual(PieceColor.Black, handNest.PieceColor);
      Assert.AreEqual(PieceType.歩, handNest.PieceType);
      Assert.AreEqual(4, handNest.PiecesCount);
    }
    [TestMethod]
    public void ChangeGroupingModeTest()
    {
      var board = new Board();
      var hand = new ShogiHand { Color = PieceColor.Black, Board = board };
      board.Black.AddToHand(PieceType.歩);
      board.Black.AddToHand(PieceType.歩);
      board.Black.AddToHand(PieceType.歩);
      board.Black.AddToHand(PieceType.歩);
      Assert.AreEqual(4, hand.Items.Count);
      hand.IsGrouping = true;
      Assert.AreEqual(1, hand.Items.Count);
      hand.IsGrouping = false;
      Assert.AreEqual(4, hand.Items.Count);
    }
  }
}