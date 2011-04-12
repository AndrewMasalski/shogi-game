using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Chess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Vector = Chess.Vector;

namespace UniTests
{
  [TestClass]
  public class BoardPanelTest
  {
    private BoardPanel _panel;
    private List<Mock<UIElement>> _mocks;

    [TestInitialize]
    public void Init()
    {
      _panel = new BoardPanel();
      _mocks = new List<Mock<UIElement>>();
    }
    [TestMethod]
    public void LayoutSingleCellFit()
    {
      SetupObject(new Vector(3, 2), new Size(10, 11), new Rect(30, 22, 10, 11));
      _panel.Measure(new Size(80, 88));
      _panel.Arrange(new Rect(1, 3, 80, 88));
      Verify();
    }
    [TestMethod]
    public void LayoutLeftEdgeCell()
    {
      SetupObject(new Vector(0, 2), new Size(40, 0), new Size(7, 9), new Rect(0, 22, 7, 11));
      _panel.Measure(new Size(80, 88));
      _panel.Arrange(new Rect(1, 3, 80, 88));
      Verify();
    }
    [TestMethod]
    public void LayoutRightEdgeCell()
    {
      SetupObject(new Vector(9, 2), new Size(40, 0), new Size(7, 9), new Rect(73, 22, 7, 11));
      _panel.Measure(new Size(80, 88));
      _panel.Arrange(new Rect(1, 3, 80, 88));
      Verify();
    }

    private void SetupObject(Vector pos, Size expectedMeasuredSize, Rect expectedArrangeRect)
    {
      SetupObject(pos, expectedMeasuredSize, expectedMeasuredSize, expectedArrangeRect);
    }
    private void SetupObject(Vector pos, Size expectedMeasuredSize, Size measuredSize, Rect expectedArrangeRect)
    {
      var m = new Mock<UIElement>();
      BoardPanel.SetColumn(m.Object, pos.X);
      BoardPanel.SetRow(m.Object, pos.Y);
      _panel.Children.Add(m.Object);

      m.SetupMeasure(s => Assert.AreEqual(expectedMeasuredSize, s))
        .Returns(measuredSize)
        .Verifiable();

      m.SetupArrange(s => Assert.AreEqual(expectedArrangeRect, s))
        .Verifiable();

      _mocks.Add(m);
    }
    private void Verify()
    {
      _mocks.ForEach(m => m.Verify());
    }
  }
}