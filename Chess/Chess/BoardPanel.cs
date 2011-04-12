using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Chess
{
  public class BoardPanel : Panel
  {
    public BoardPanel()
    {
      _elements = new List<UIElement>[(int)Pos.Count];

      _elements[(int)Pos.Background] = new List<UIElement>();

      _elements[(int)Pos.Cell] = new List<UIElement>(64);

      _elements[(int)Pos.Row] = new List<UIElement>(9);
      _elements[(int)Pos.Column] = new List<UIElement>(9);

      _elements[(int)Pos.TopEdgeCell] = new List<UIElement>(8);
      _elements[(int)Pos.LeftEdgeCell] = new List<UIElement>(8);
      _elements[(int)Pos.RightEdgeCell] = new List<UIElement>(8);
      _elements[(int)Pos.BottomEdgeCell] = new List<UIElement>(8);

      _elements[(int)Pos.TopLeftCorner] = new List<UIElement>();
      _elements[(int)Pos.TopRightCorner] = new List<UIElement>();
      _elements[(int)Pos.BottomLeftCorner] = new List<UIElement>();
      _elements[(int)Pos.BottomRightCorner] = new List<UIElement>();
    }

    #region ' Column : int Attached Property '

    public static readonly DependencyProperty ColumnProperty = DependencyProperty.
      RegisterAttached("Column", typeof(int), typeof(BoardPanel),
                       new FrameworkPropertyMetadata(-1, OnColumnChanged, CoerceColumnProperty));

    private static object CoerceColumnProperty(DependencyObject d, object basevalue)
    {
      var value = (int)basevalue;
      if (value < -1 || value > 9) return -1;
      return basevalue;
    }

    public static int GetColumn(DependencyObject obj)
    {
      return (int)obj.GetValue(ColumnProperty);
    }

    public static void SetColumn(DependencyObject obj, int value)
    {
      obj.SetValue(ColumnProperty, value);
    }

    private static void OnColumnChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      var element = obj as UIElement;
      if (element == null) return;
      var panel = LogicalTreeHelper.GetParent(element) as BoardPanel;
      if (panel == null) return;
      var row = GetRow(element);
      panel.Update(element,
        (int)args.OldValue, row,
        (int)args.NewValue, row);
    }

    #endregion

    #region ' Row : int Attached Property '

    public static readonly DependencyProperty RowProperty = DependencyProperty.
      RegisterAttached("Row", typeof(int), typeof(BoardPanel),
                       new FrameworkPropertyMetadata(-1, OnRowChanged, CoerceRowProperty));

    private static object CoerceRowProperty(DependencyObject d, object basevalue)
    {
      var value = (int)basevalue;
      if (value < -1 || value > 9) return -1;
      return basevalue;
    }

    public static int GetRow(DependencyObject obj)
    {
      return (int)obj.GetValue(RowProperty);
    }

    public static void SetRow(DependencyObject obj, int value)
    {
      obj.SetValue(RowProperty, value);
    }

    private static void OnRowChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      var element = obj as UIElement;
      if (element == null) return;
      var panel = LogicalTreeHelper.GetParent(element) as BoardPanel;
      if (panel == null) return;
      var column = GetColumn(element);
      panel.Update(element,
        column, (int)args.OldValue,
        column, (int)args.NewValue);
    }

    #endregion

    #region ' Elements categorization '

    protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
    {
      var added = visualAdded as UIElement;
      if (added != null)
      {
        Add(added);
      }
      var removed = visualRemoved as UIElement;
      if (removed != null)
      {
        Remove(removed);
      }
    }
    private void Update(UIElement element, int oldColumn, int oldRow, int newColumn, int newRow)
    {
      _elements[(int)GetPos(oldColumn, oldRow)].Remove(element);
      _elements[(int)GetPos(newColumn, newRow)].Add(element);
    }
    private void Add(UIElement element)
    {
      _elements[(int)GetPos(GetColumn(element), GetRow(element))].Add(element);
    }
    private void Remove(UIElement element)
    {
      _elements[(int)GetPos(GetColumn(element), GetRow(element))].Remove(element);
    }

    #endregion

    private enum Pos
    {
      Background,
      Cell,
      Row, Column,
      TopEdgeCell, RightEdgeCell, LeftEdgeCell, BottomEdgeCell,
      TopLeftCorner, TopRightCorner, BottomLeftCorner, BottomRightCorner,
      Count
    }

    private readonly List<UIElement>[] _elements;

    #region ' Measure '

    protected override Size MeasureOverride(Size availableSize)
    {
      Measure(Pos.Background, availableSize);
      MeasureCells(availableSize);
      MeasureRowsAndCols(availableSize);
      MeasureEdgeCells(availableSize);
      MeasureCorners();

      return GetSize();
    }

    private void MeasureCells(Size availableSize)
    {
      Measure(Pos.Cell, new Size(availableSize.Width / 8, availableSize.Height / 8));

      _cellsWidth = Max(_elements[(int)Pos.Cell], c => c.DesiredSize.Width) * 8;
      _cellsHeight = Max(_elements[(int)Pos.Cell], c => c.DesiredSize.Height) * 8;
    }
    private void MeasureRowsAndCols(Size availableSize)
    {
      var widthLeft = Math.Max(availableSize.Width - _cellsWidth, 0);
      var heightLeft = Math.Max(availableSize.Height - _cellsHeight, 0);

      var rowConstraint = heightLeft / 9;
      var columnConstraint = widthLeft / 9;

      _elements[(int)Pos.Row].ForEach(row => row.Measure(new Size(availableSize.Width, rowConstraint)));
      _elements[(int)Pos.Column].ForEach(column => column.Measure(new Size(columnConstraint, availableSize.Height)));

      _rowsHeight = Sum(_elements[(int)Pos.Row], c => c.DesiredSize.Width);
      _columnsWidth = Sum(_elements[(int)Pos.Column], c => c.DesiredSize.Height);
    }
    private void MeasureEdgeCells(Size availableSize)
    {
      var widthLeft = Math.Max(availableSize.Width - _cellsWidth - _columnsWidth, 0);
      var heightLeft = Math.Max(availableSize.Height - _cellsHeight - _rowsHeight, 0);

      var horizEdgeCellsConstraint = new Size(_cellsWidth / 8, heightLeft / 2);
      var vertEdgeCellsConstraint = new Size(widthLeft / 2, _cellsHeight / 8);

      var topEdgeCells = _elements[(int)Pos.TopEdgeCell];
      var bottomEdgeCells = _elements[(int)Pos.BottomEdgeCell];
      var leftEdgeCells = _elements[(int)Pos.LeftEdgeCell];
      var rightEdgeCells = _elements[(int)Pos.RightEdgeCell];

      topEdgeCells.ForEach(cell => cell.Measure(horizEdgeCellsConstraint));
      bottomEdgeCells.ForEach(cell => cell.Measure(horizEdgeCellsConstraint));
      leftEdgeCells.ForEach(cell => cell.Measure(vertEdgeCellsConstraint));
      rightEdgeCells.ForEach(cell => cell.Measure(vertEdgeCellsConstraint));

      _topEdgeCellsHeight = Max(topEdgeCells, cell => cell.DesiredSize.Height);
      _bottomEdgeCellsHeight = Max(bottomEdgeCells, cell => cell.DesiredSize.Height);
      _leftEdgeCellsWidth = Max(leftEdgeCells, cell => cell.DesiredSize.Width);
      _rightEdgeCellsWidth = Max(rightEdgeCells, cell => cell.DesiredSize.Width);
    }
    private void MeasureCorners()
    {
      Measure(Pos.TopLeftCorner, new Size(_leftEdgeCellsWidth, _topEdgeCellsHeight));
      Measure(Pos.TopRightCorner, new Size(_rightEdgeCellsWidth, _topEdgeCellsHeight));
      Measure(Pos.BottomLeftCorner, new Size(_leftEdgeCellsWidth, _bottomEdgeCellsHeight));
      Measure(Pos.BottomRightCorner, new Size(_rightEdgeCellsWidth, _bottomEdgeCellsHeight));
    }
    private void Measure(Pos pos, Size constraint)
    {
      _elements[(int)pos].ForEach(c => c.Measure(constraint));
    }
    private Size GetSize()
    {
      return new Size(
        _cellsWidth + _columnsWidth + _leftEdgeCellsWidth + _rightEdgeCellsWidth,
        _cellsHeight + _rowsHeight + _topEdgeCellsHeight + _bottomEdgeCellsHeight);
    }

    #endregion

    private double _cellsWidth;
    private double _columnsWidth;
    private double _leftEdgeCellsWidth;
    private double _rightEdgeCellsWidth;
    private double _cellsHeight;
    private double _rowsHeight;
    private double _topEdgeCellsHeight;
    private double _bottomEdgeCellsHeight;

    #region ' Arrange '

    protected override Size ArrangeOverride(Size arrangeSize)
    {
      AdjustMeasures(arrangeSize);
      ArrangeBackground(arrangeSize);
      ArrangeCorners();
      ArrangeEdges();
      AdjustRowsAndColumns();
      ArrangeRowsAndColumns(arrangeSize);
      ArrangeCells();
      return arrangeSize;
    }
    private void AdjustMeasures(Size arrangeSize)
    {
      var oldColumnsWidth = _columnsWidth;
      var oldRowsHeight = _rowsHeight;

      //-------------------------------------------------------

      var excessiveWidth = arrangeSize.Width -
        _cellsWidth - _columnsWidth - _leftEdgeCellsWidth - _rightEdgeCellsWidth;
      if (excessiveWidth >= 0)
      {
        _cellsWidth += excessiveWidth;
      }
      else
      {
        if (-excessiveWidth > _leftEdgeCellsWidth + _rightEdgeCellsWidth)
        {
          excessiveWidth += _leftEdgeCellsWidth + _rightEdgeCellsWidth;
        }
        else
        {
          _leftEdgeCellsWidth = _rightEdgeCellsWidth = -excessiveWidth/2;
          excessiveWidth = 0;
        }
        _columnsWidth = excessiveWidth;
      }

      //-------------------------------------------------------

      var excessiveHeight = arrangeSize.Height -
        _cellsHeight - _rowsHeight - _topEdgeCellsHeight - _bottomEdgeCellsHeight;
      if (excessiveHeight >= 0)
      {
        _cellsHeight += excessiveHeight;
      }
      else
      {
        if (-excessiveHeight > _topEdgeCellsHeight + _bottomEdgeCellsHeight)
        {
          excessiveHeight += _topEdgeCellsHeight + _bottomEdgeCellsHeight;
        }
        else
        {
          _topEdgeCellsHeight = _bottomEdgeCellsHeight = -excessiveHeight / 2;
          excessiveHeight = 0;
        }
        _rowsHeight = excessiveHeight;
      }

      //-------------------------------------------------------

      _columnsWidthRatio = 1;
      _rowsHeightRatio = 1;

      if (oldColumnsWidth > 0) _columnsWidthRatio = _columnsWidth / oldColumnsWidth;
      if (oldRowsHeight > 0) _rowsHeightRatio = _rowsHeight / oldRowsHeight;
    }
    private void ArrangeBackground(Size arrangeSize)
    {
      foreach (var e in _elements[(int)Pos.Background])
        e.Arrange(new Rect(new Point(), arrangeSize));
    }
    private void ArrangeCorners()
    {
      foreach (var e in _elements[(int)Pos.BottomLeftCorner])
        e.Arrange(new Rect(new Point(),
                           new Size(_leftEdgeCellsWidth, _topEdgeCellsHeight)));

      foreach (var e in _elements[(int)Pos.TopLeftCorner])
        e.Arrange(new Rect(new Point(0,
                                     _cellsHeight + _rowsHeight + _topEdgeCellsHeight),
                           new Size(_leftEdgeCellsWidth, _bottomEdgeCellsHeight)));

      foreach (var e in _elements[(int)Pos.TopRightCorner])
        e.Arrange(new Rect(new Point(
                             _cellsWidth + _columnsWidth + _leftEdgeCellsWidth,
                             _cellsHeight + _rowsHeight + _topEdgeCellsHeight),
                           new Size(_rightEdgeCellsWidth, _bottomEdgeCellsHeight)));

      foreach (var e in _elements[(int)Pos.BottomRightCorner])
        e.Arrange(new Rect(new Point(
                             _cellsWidth + _columnsWidth + _leftEdgeCellsWidth, 0),
                           new Size(_rightEdgeCellsWidth, _topEdgeCellsHeight)));
    }
    private void ArrangeEdges()
    {
      foreach (var e in _elements[(int)Pos.LeftEdgeCell])
        e.Arrange(new Rect(
          new Point(0, _topEdgeCellsHeight + _cellsHeight / 8 * GetRow(e)),
          new Size(_leftEdgeCellsWidth, _cellsHeight / 8)));

      foreach (var e in _elements[(int)Pos.TopEdgeCell])
        e.Arrange(new Rect(
          new Point(_leftEdgeCellsWidth + _columnsWidth / 8 * GetColumn(e), 0),
          new Size(_columnsWidth / 8, _topEdgeCellsHeight)));

      foreach (var e in _elements[(int)Pos.RightEdgeCell])
        e.Arrange(new Rect(
          new Point(_cellsWidth + _columnsWidth + _leftEdgeCellsWidth,
            _topEdgeCellsHeight + _cellsHeight / 8 * GetRow(e)),
            new Size(_rightEdgeCellsWidth, _cellsHeight / 8)));

      foreach (var e in _elements[(int)Pos.BottomEdgeCell])
        e.Arrange(new Rect(
          new Point(_leftEdgeCellsWidth + _columnsWidth / 8 * GetColumn(e),
            _cellsHeight + _rowsHeight + _topEdgeCellsHeight),
            new Size(_columnsWidth / 8, _topEdgeCellsHeight)));
    }
    private void AdjustRowsAndColumns()
    {
      _cols.Initialize();
      _rows.Initialize();
      _sumWs.Initialize();
      _sumHs.Initialize();

      foreach (var e in _elements[(int)Pos.Column])
      {
        var col = GetColumn(e);
        _cols[col] = Math.Max(_cols[col], e.DesiredSize.Width);
      }

      foreach (var e in _elements[(int)Pos.Row])
      {
        var row = GetRow(e);
        _rows[row] = Math.Max(_rows[row], e.DesiredSize.Height);
      }

      for (int i = 0; i < 9; i++)
      {
        _cols[i] *= _columnsWidthRatio;
        _rows[i] *= _rowsHeightRatio;
      }

      _sumWs[0] = _cols[0] + _leftEdgeCellsWidth;
      _sumHs[0] = _rows[0] + _topEdgeCellsHeight;

      for (int i = 1; i < 9; i++)
      {
        _sumWs[i] = _sumWs[i - 1] + _cellsWidth / 8 + _cols[i];
        _sumHs[i] = _sumHs[i - 1] + _cellsHeight / 8 + _rows[i];
      }
    }
    private void ArrangeRowsAndColumns(Size arrangeSize)
    {
      foreach (var e in _elements[(int)Pos.Column])
      {
        var col = GetColumn(e);
        var width = _cols[col];
        var left = _sumWs[col] - width;
        e.Arrange(new Rect(new Point(left, 0), new Size(width, arrangeSize.Height)));
      }

      foreach (var e in _elements[(int)Pos.Row])
      {
        var row = GetRow(e);
        var height = _rows[row];
        var top = _sumHs[row] - height;
        e.Arrange(new Rect(new Point(0, top), new Size(arrangeSize.Width, height)));
      }
    }
    private void ArrangeCells()
    {
      foreach (var e in _elements[(int)Pos.Cell])
      {
        var col = GetColumn(e);
        var row = GetRow(e);
        e.Arrange(new Rect(
          new Point(_sumWs[col], _sumHs[row]),
          new Size(_cellsWidth/8, _cellsHeight/8)));
      }
    }

    #endregion

    private double _columnsWidthRatio;
    private double _rowsHeightRatio;

    private readonly double[] _rows = new double[9];
    private readonly double[] _cols = new double[9];
    private readonly double[] _sumWs = new double[9];
    private readonly double[] _sumHs = new double[9];

    private static Pos GetPos(int c, int r)
    {
      if (c == -1 && r == -1) return Pos.Background;

      if (c >= 1 && c <= 8 && r >= 1 && r <= 8) return Pos.Cell;

      if (c == 0 && (r >= 1 && r <= 8)) return Pos.LeftEdgeCell;
      if (c == 9 && (r >= 1 && r <= 8)) return Pos.RightEdgeCell;
      if (r == 0 && (c >= 1 && c <= 8)) return Pos.BottomEdgeCell;
      if (r == 9 && (c >= 1 && c <= 8)) return Pos.TopEdgeCell;

      if (c == 0 && r == 0) return Pos.BottomLeftCorner;
      if (c == 0 && r == 9) return Pos.TopLeftCorner;
      if (c == 9 && r == 0) return Pos.BottomRightCorner;
      if (c == 9 && r == 9) return Pos.TopRightCorner;

      if (c == -1 && (r >= 0 && r <= 9)) return Pos.Row;
      if (r == -1 && (c >= 0 && c <= 9)) return Pos.Column;

      throw new Exception("The wrong value should have been coerced!");
    }

    private static double Max(IEnumerable<UIElement> src, Func<UIElement, double> fetch)
    {
      return !src.Any() ? 0 : src.Max(fetch);
    }
    private static double Sum(IEnumerable<UIElement> src, Func<UIElement, double> fetch)
    {
      return !src.Any() ? 0 : src.Sum(fetch);
    }
  }
}