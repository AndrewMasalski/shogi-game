using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.BoardControl.Controls;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Primitives;

namespace BoardControl.UnitTests.Controls
{
  /*  _________________________________
   * 
   *  That's what we're gonna test:
   *  _________________________________
   * 
   *  var nest = new HandNest();
   *  nest.DetachPiece();
   *  nest.PiecesCount = 1;
   *  nest.PieceColor = PieceColor.Black;
   *  nest.PieceType = PT.馬;
   *  nest.IsFlipped = true;
   *  nest.IsMoveSource = true;
   *  nest.IsPossibleMoveTarget = true;
   *  Verify(nest.Content)
   *  __________________________________
   */
  [TestClass]
  public class HandNestTest
  {
    [TestMethod]
    public void SimplePropertiesTest()
    {
      var nest = new HandNest();
      Assert.IsNotNull(nest.Content);
      nest.PieceType = PT.香;
      Assert.AreEqual(PT.香, nest.ShogiPiece.PieceType);
      nest.PieceType = PT.と;
      Assert.AreEqual(PT.と, nest.ShogiPiece.PieceType);
      nest.PieceColor = PieceColor.White;
      Assert.AreEqual(PieceColor.White, nest.ShogiPiece.PieceColor);
      nest.PieceColor = PieceColor.Black;
      Assert.AreEqual(PieceColor.Black, nest.ShogiPiece.PieceColor);
      nest.IsFlipped = true;
      Assert.IsTrue(nest.ShogiPiece.IsFlipped);
      nest.IsFlipped = false;
      Assert.IsFalse(nest.ShogiPiece.IsFlipped);
      nest.IsMoveSource = true;
      Assert.IsTrue(nest.IsMoveSource);
      nest.IsMoveSource = false;
      Assert.IsFalse(nest.IsMoveSource);
      nest.IsPossibleMoveTarget = false;
      Assert.IsFalse(nest.IsPossibleMoveTarget);
      nest.IsPossibleMoveTarget = true;
      Assert.IsTrue(nest.IsPossibleMoveTarget);
    }
    [TestMethod]
    public void DetachTest()
    {
      var nest = new HandNest();
      Assert.AreEqual(PT.王, nest.ShogiPiece.PieceType);
      Assert.AreEqual(PieceColor.Black, nest.ShogiPiece.PieceColor);
      var piece = nest.DetachPiece();
      Assert.IsNull(nest.ShogiPiece);
      Assert.AreEqual(PT.王, piece.PieceType);
      Assert.AreEqual(PieceColor.Black, piece.PieceColor);
    }
  }
}
