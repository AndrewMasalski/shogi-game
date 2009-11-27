using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RulesVisualization;
using Yasc.ShogiCore;

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
      var assembly = typeof(PieceTest).Assembly;
      foreach (var page in assembly.ActivateObjectsOfType<Page>("Positions"))
      {
        var diagram = (BoardDiagram)page.Content;
        Assert.AreEqual("玉", (string)diagram.Board["1i"].Type);
      }
    }
  }
}