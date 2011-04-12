using System.Windows;
using Chess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UniTests
{
  [TestClass]
  public class BoardPanelTest
  {
    [TestMethod]
    public void Ctor()
    {
      var p = new BoardPanel();
      var m = new Mock<UIElement>();
      BoardPanel.SetRow(m.Object, 2);
      BoardPanel.SetColumn(m.Object, 3);
      p.Children.Add(m.Object);

      m.SetupMeasure(s => Assert.AreEqual(new Size(10, 10), s))
        .Returns(new Size(10, 10))
        .Verifiable();

      p.Measure(new Size(80, 80));

      m.SetupArrange(s => Assert.AreEqual(new Rect(61, 33, 10, 10), s))
        .Verifiable();

      p.Arrange(new Rect(new Point(1, 3), new Size(80, 80)));

      m.Verify();
    }
  }
}