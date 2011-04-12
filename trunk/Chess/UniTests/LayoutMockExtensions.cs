using System;
using System.Diagnostics;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Language.Flow;
using Moq.Protected;

namespace UniTests
{
  [DebuggerStepThrough]
  public static class LayoutMockExtensions
  {
    private const string MeasureCore = "MeasureCore";

    public static ISetup<UIElement, Size> SetupMeasure(this Mock<UIElement> mock, Size expectedValue)
    {
      var areEqual = new Func<Size, Size, bool>(
        (exp, act) =>
          {
            Assert.AreEqual(exp, act, "MeasureCore was called with " + act + " where " + exp + " was expected");
            return true;
          });

      return mock.Protected()
        .Setup<Size>(MeasureCore, ItExpr.Is((Size s) => areEqual(expectedValue, s)));
    }
    public static ISetup<UIElement, Size> SetupMeasure(this Mock<UIElement> mock, Action<Size> expectedValue)
    {
      var areEqual = new Func<Size, bool>(
        s =>
          {
            expectedValue(s);
            return true;
          });

      return mock.Protected()
        .Setup<Size>(MeasureCore, ItExpr.Is((Size s) => areEqual(s)));
    }
    
    private const string ArrangeCore = "ArrangeCore";

    public static ISetup<UIElement> SetupArrange(this Mock<UIElement> mock, Rect expectedValue)
    {
      var areEqual = new Func<Rect, Rect, bool>(
        (exp, act) =>
          {
            Assert.AreEqual(exp, act, "ArrangeCore was called with " + act + " where " + exp + " was expected");
            return true;
          });

      return mock.Protected()
        .Setup(ArrangeCore, ItExpr.Is((Rect s) => areEqual(expectedValue, s)));
    }
    public static ISetup<UIElement> SetupArrange(this Mock<UIElement> mock, Action<Rect> expectedValue)
    {
      var areEqual = new Func<Rect, bool>(
        s =>
          {
            expectedValue(s);
            return true;
          });

      return mock.Protected()
        .Setup(ArrangeCore, ItExpr.Is((Rect s) => areEqual(s)));
    }
  }
}