using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.BoardControl.Controls;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.Primitives;

namespace BoardControl.UnitTests.Controls
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
      Assert.AreEqual(HandGroupingMode.Plain, hand.GroupingMode);
      Assert.AreEqual(0, hand.Items.Count);
    }
    [TestMethod]
    public void FlatTest()
    {
      var board = new Board();
      var hand = new ShogiHand { Color = PieceColor.Black, Board = board};
      board.Black.Hand.Add(PieceType.馬);
      Assert.AreEqual(PieceType.角, hand.Items[0].PieceType);
      Assert.AreEqual(PieceColor.Black, hand.Items[0].PieceColor);

      hand.Color = PieceColor.White;
      Assert.AreEqual(0, hand.Items.Count);
      Assert.AreSame(board.White.Hand, hand.Hand);
      board.White.Hand.Add(PieceType.歩);
      board.White.Hand.Add(PieceType.歩);
      board.White.Hand.Add(PieceType.歩);
      board.White.Hand.Add(PieceType.歩);
      Assert.AreEqual(4, hand.Items.Count);
      Assert.IsTrue(board.White.Hand.Remove(PieceType.歩));
      Assert.AreEqual(3, hand.Items.Count);
    }
    [TestMethod]
    public void FlatTestNestDetails()
    {
      var board = new Board();
      var hand = new ShogiHand { Color = PieceColor.Black, Board = board};
      board.Black.Hand.Add(PieceType.歩);
      board.Black.Hand.Add(PieceType.歩);
      board.Black.Hand.Add(PieceType.歩);
      board.Black.Hand.Add(PieceType.歩);
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
                     GroupingMode = HandGroupingMode.Groups, 
                     Board = board
                   };
      board.Black.Hand.Add(PieceType.歩);
      board.Black.Hand.Add(PieceType.歩);
      board.Black.Hand.Add(PieceType.歩);
      board.Black.Hand.Add(PieceType.歩);
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
      var hand = new ShogiHand { Color = PieceColor.White, Board = board };
      board.White.Hand.Add(PieceType.歩);
      board.White.Hand.Add(PieceType.歩);
      board.White.Hand.Add(PieceType.歩);
      board.White.Hand.Add(PieceType.歩);
      Assert.AreEqual(4, hand.Items.Count);
      hand.GroupingMode = HandGroupingMode.Groups;
      Assert.AreEqual(1, hand.Items.Count);
      hand.GroupingMode = HandGroupingMode.Plain;
      Assert.AreEqual(4, hand.Items.Count);
    }
    [TestMethod]
    public void TestOrderedGroupsMode()
    {
      var board = new Board();
      var hand = new ShogiHand
                   {
                     Color = PieceColor.Black,
                     GroupingMode = HandGroupingMode.OrderedGroups,
                     Board = board
                   };
      board.Black.Hand.Add(PieceType.桂);
      board.Black.Hand.Add(PieceType.桂);
      board.Black.Hand.Add(PieceType.桂);
      board.Black.Hand.Add(PieceType.桂);
      Assert.AreEqual(9, hand.Items.Count);

      Assert.AreEqual(PieceType.王, hand.Items[0].PieceType);
      Assert.AreEqual(PieceType.玉, hand.Items[1].PieceType);
      Assert.AreEqual(PieceType.飛, hand.Items[2].PieceType);
      Assert.AreEqual(PieceType.角, hand.Items[3].PieceType);
      Assert.AreEqual(PieceType.金, hand.Items[4].PieceType);
      Assert.AreEqual(PieceType.銀, hand.Items[5].PieceType);
      Assert.AreEqual(PieceType.桂, hand.Items[6].PieceType);
      Assert.AreEqual(PieceType.香, hand.Items[7].PieceType);
      Assert.AreEqual(PieceType.歩, hand.Items[8].PieceType);

      Assert.AreEqual(0, hand.Items[0].PiecesCount);
      Assert.AreEqual(0, hand.Items[1].PiecesCount);
      Assert.AreEqual(0, hand.Items[2].PiecesCount);
      Assert.AreEqual(0, hand.Items[3].PiecesCount);
      Assert.AreEqual(0, hand.Items[4].PiecesCount);
      Assert.AreEqual(0, hand.Items[5].PiecesCount);
      Assert.AreEqual(4, hand.Items[6].PiecesCount);
      Assert.AreEqual(0, hand.Items[7].PiecesCount);
      Assert.AreEqual(0, hand.Items[8].PiecesCount);
    }

    [TestMethod]
    public void TestOrderedGroupsModeInAction()
    {
      var board = new Board();
      var hand = new ShogiHand
      {
        Color = PieceColor.Black,
        GroupingMode = HandGroupingMode.OrderedGroups,
        Board = board
      };
      Assert.AreEqual(9, hand.Items.Count);
      Assert.AreEqual(0, hand.Items[5].PiecesCount);
      Assert.AreEqual(0, hand.Items[6].PiecesCount);
      
      board.Black.Hand.Add(PieceType.桂);
      Assert.AreEqual(0, hand.Items[5].PiecesCount);
      Assert.AreEqual(1, hand.Items[6].PiecesCount);

      board.Black.Hand.Add(PieceType.桂);
      Assert.AreEqual(0, hand.Items[5].PiecesCount);
      Assert.AreEqual(2, hand.Items[6].PiecesCount);

      board.Black.Hand.Remove(PieceType.桂);
      Assert.AreEqual(0, hand.Items[5].PiecesCount);
      Assert.AreEqual(1, hand.Items[6].PiecesCount);

      board.Black.Hand.Remove(PieceType.桂);
      Assert.AreEqual(0, hand.Items[5].PiecesCount);
      Assert.AreEqual(0, hand.Items[6].PiecesCount);

      Assert.AreEqual(9, hand.Items.Count);
    }
  }
}