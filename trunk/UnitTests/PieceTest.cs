using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.Diagram;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Moves;

namespace UnitTests
{
  [TestClass]
  public class PieceTest
  {
    [TestMethod]
    public void TestPromote()
    {
      var b = new Board();
      var p = new Piece(b.White, "歩");
      p.IsPromoted = true;
      Assert.AreEqual("と", (string)p.Type);
      p.IsPromoted = false;
      Assert.AreEqual("歩", (string)p.Type);
    }
    [TestMethod]
    public void DiagramTest()
    {
      foreach (var uri in typeof(PieceTest).Assembly.GetBamls())
      {
        var p = (Page)Application.LoadComponent(uri);
        var m = (ValidMovesManifest) p.FindResource("ValidMovesManifest");
        IEnumerable<MoveBase> moves;
        foreach (var item in m.Content)
        {
//          moves = Parse(item);
          
        }
        var diagram = (BoardDiagram)p.Content;
        Assert.AreEqual("玉", (string)diagram.Board["1i"].Type);
        
      }
    }
  }
}