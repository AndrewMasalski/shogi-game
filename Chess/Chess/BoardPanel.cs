using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Chess
{
  public class BoardPanel : Panel
  {
    #region ' Common '

    delegate T Aggregate<T>(IEnumerable<UIElement> src, Func<UIElement, T> fetch);

    private enum Pos
    {
      Background,
      Cell,
      Row, Column,
      TopEdgeCell, RightEdgeCell, LeftEdgeCell, BottomEdgeCell,
      TopLeftCorner, TopRightCorner, BottomLeftCorner, BottomRightCorner,
      Unknown,
      Count
    }

    private readonly List<UIElement>[] _elements;
    private static readonly Size InfiniteSize =
      new Size(double.PositiveInfinity, double.PositiveInfinity);

    #endregion

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
      
      _elements[(int)Pos.Unknown] = new List<UIElement>();
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

    #region ' Measure Fields '

    private double _cellSize;
    private double _columnsWidth;
    private double _leftEdgeCellsWidth;
    private double _rightEdgeCellsWidth;
    private double _rowsHeight;
    private double _topEdgeCellsHeight;
    private double _bottomEdgeCellsHeight;

    private bool _isColumnAndRowVisible;
    private bool _isEdgeVisible;

    #endregion

    #region ' Measure '

    protected override Size MeasureOverride(Size availableSize)
    {
      _cellSize = 0;
      _columnsWidth = 0;
      _leftEdgeCellsWidth = 0;
      _rightEdgeCellsWidth = 0;
      _rowsHeight = 0;
      _topEdgeCellsHeight = 0;
      _bottomEdgeCellsHeight = 0;

      _isColumnAndRowVisible = false;
      _isEdgeVisible = false;

      //     ____________
      //____/ Background \____________________________________________________
      Measure(Pos.Background, availableSize, Max, e => e.DesiredSize);

      //     _______
      //____/ Cells \_________________________________________________________
      var max = Measure(Pos.Cell, Max, c => c.DesiredSize);
      _cellSize = Math.Max(max.Width, max.Height);
      _isColumnAndRowVisible =
        _cellSize * 8 < availableSize.Height &&
        _cellSize * 8 < availableSize.Width;

      //     ________________
      //____/ Rows & Columns \________________________________________________
      if (_isColumnAndRowVisible)
      {
        // bug: don't sum up two values for the same column
        _rowsHeight = Measure(Pos.Row, Sum, c => c.DesiredSize.Height);
        _columnsWidth = Measure(Pos.Column, Sum, c => c.DesiredSize.Width);

        _isEdgeVisible =
          _cellSize * 8 + _rowsHeight < availableSize.Height &&
          _cellSize * 8 + _columnsWidth < availableSize.Width;
      }

      //     _______
      //____/ Edges \_________________________________________________________
      if (_isEdgeVisible)
      {
        _topEdgeCellsHeight = Measure(Pos.TopEdgeCell, Max, c => c.DesiredSize.Height);
        _bottomEdgeCellsHeight = Measure(Pos.BottomEdgeCell, Max, c => c.DesiredSize.Height);
        _leftEdgeCellsWidth = Measure(Pos.LeftEdgeCell, Max, c => c.DesiredSize.Width);
        _rightEdgeCellsWidth = Measure(Pos.RightEdgeCell, Max, c => c.DesiredSize.Width);

        //     _________
        //____/ Corners \_____________________________________________________
        var topLeft = Measure(Pos.TopLeftCorner, Max, c => c.DesiredSize);
        var topRight = Measure(Pos.TopRightCorner, Max, c => c.DesiredSize);
        var bottomLeft = Measure(Pos.BottomLeftCorner, Max, c => c.DesiredSize);
        var bottomRight = Measure(Pos.BottomRightCorner, Max, c => c.DesiredSize);

        _topEdgeCellsHeight = Math.Max(
          _topEdgeCellsHeight, Math.Max(topLeft.Width, topRight.Width));

        _bottomEdgeCellsHeight = Math.Max(
          _bottomEdgeCellsHeight, Math.Max(bottomLeft.Width, bottomRight.Width));

        _leftEdgeCellsWidth = Math.Max(
          _leftEdgeCellsWidth, Math.Max(topLeft.Width, bottomLeft.Width));

        _rightEdgeCellsWidth = Math.Max(
          _rightEdgeCellsWidth, Math.Max(topRight.Width, bottomRight.Width));
      }

      return GetSize();
    }
    private Size GetSize()
    {
      return new Size(
        _cellSize * 8 + _columnsWidth + _leftEdgeCellsWidth + _rightEdgeCellsWidth,
        _cellSize * 8 + _rowsHeight + _topEdgeCellsHeight + _bottomEdgeCellsHeight);
    }

    #endregion

    #region ' Arrange Fields '

    private double _spearHeight;
    private double _spearWidth;
    private readonly double[] _rows = new double[9];
    private readonly double[] _cols = new double[9];

    #endregion

    #region ' Arrange '

    protected override Size ArrangeOverride(Size arrangeSize)
    {
      AdjustMeasures(arrangeSize);
      AdjustRowsAndColumns();

      ArrangeBackground(arrangeSize);
      ArrangeCorners();
      ArrangeEdges();
      ArrangeRowsAndColumns(arrangeSize);
      ArrangeCells();
      return arrangeSize;
    }
    private void AdjustMeasures(Size arrangeSize)
    {
      var vertEdgesWidth = _leftEdgeCellsWidth + _rightEdgeCellsWidth;
      var horizEdgesHeight = _topEdgeCellsHeight + _bottomEdgeCellsHeight;

      //-------------------------------------------------------

      if (_cellSize * 8 + _columnsWidth + vertEdgesWidth > arrangeSize.Width)
      {
        _leftEdgeCellsWidth = _rightEdgeCellsWidth = vertEdgesWidth = 0;
        _isEdgeVisible = false;
      }
      if (_cellSize * 8 + _columnsWidth > arrangeSize.Width)
      {
        _columnsWidth = _rowsHeight = 0;
        _isColumnAndRowVisible = false;
      }

      //-------------------------------------------------------

      if (_cellSize * 8 + _rowsHeight + horizEdgesHeight > arrangeSize.Height)
      {
        _topEdgeCellsHeight = _bottomEdgeCellsHeight = horizEdgesHeight = 0;
        _isEdgeVisible = false;
      }
      if (_cellSize * 8 + _rowsHeight > arrangeSize.Height)
      {
        _columnsWidth = _rowsHeight = 0;
        _isColumnAndRowVisible = false;
      }

      //-------------------------------------------------------

      var w = arrangeSize.Width - _columnsWidth - vertEdgesWidth;
      var h = arrangeSize.Height - _rowsHeight - horizEdgesHeight;

      var min = Math.Min(w, h);
      _cellSize = min / 8;
      
      _spearWidth = w - min;
      _spearHeight = h - min;
    }
    private void AdjustRowsAndColumns()
    {
      for (int i = 0; i < 9; i++)
      {
        _cols[i] = 0;
        _rows[i] = 0;
      }

      if (!_isColumnAndRowVisible) return;

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

      for (int i = 1; i < 9; i++)
      {
        _cols[i] = _cols[i - 1] + _cols[i];
        _rows[i] = _rows[i - 1] + _rows[i];
      }
    }
    private void ArrangeBackground(Size arrangeSize)
    {
      foreach (var e in _elements[(int)Pos.Background])
        e.Arrange(new Rect(
          new Point(_spearWidth / 2, _spearHeight / 2), 
          arrangeSize));
    }
    private void ArrangeCorners()
    {
      if (!_isEdgeVisible) return;

      foreach (var e in _elements[(int)Pos.TopLeftCorner])
        e.Arrange(new Rect(
          _spearWidth / 2, _spearHeight / 2, 
          _leftEdgeCellsWidth, _topEdgeCellsHeight));

      foreach (var e in _elements[(int)Pos.BottomLeftCorner])
        e.Arrange(new Rect(
          _spearWidth / 2, GetBottomRowOffset(8),
          _leftEdgeCellsWidth, _bottomEdgeCellsHeight));

      foreach (var e in _elements[(int)Pos.BottomRightCorner])
        e.Arrange(new Rect(
          GetRightColumnOffset(8), GetBottomRowOffset(8),
          _rightEdgeCellsWidth, _bottomEdgeCellsHeight));

      foreach (var e in _elements[(int)Pos.TopRightCorner])
        e.Arrange(new Rect(
          GetRightColumnOffset(8), _spearHeight / 2,
          _rightEdgeCellsWidth, _topEdgeCellsHeight));
    }
    private void ArrangeEdges()
    {
      if (!_isEdgeVisible) return;

      foreach (var e in _elements[(int)Pos.LeftEdgeCell])
        e.Arrange(new Rect(
          _spearWidth / 2, GetBottomRowOffset(GetRow(e)-1),
          _leftEdgeCellsWidth, _cellSize));

      foreach (var e in _elements[(int)Pos.TopEdgeCell])
        e.Arrange(new Rect(
          GetRightColumnOffset(GetColumn(e) - 1), _spearHeight / 2,
          _cellSize, _topEdgeCellsHeight));

      foreach (var e in _elements[(int)Pos.RightEdgeCell])
        e.Arrange(new Rect(
          GetRightColumnOffset(8), GetBottomRowOffset(GetRow(e) - 1),
          _rightEdgeCellsWidth, _cellSize));

      foreach (var e in _elements[(int)Pos.BottomEdgeCell])
        e.Arrange(new Rect(
          GetRightColumnOffset(GetColumn(e) - 1), GetBottomRowOffset(8),
          _cellSize, _bottomEdgeCellsHeight));
    }
    private void ArrangeRowsAndColumns(Size arrangeSize)
    {
      if (!_isColumnAndRowVisible) return;

      foreach (var e in _elements[(int)Pos.Column])
        e.Arrange(new Rect(
          new Point(GetLeftColumnOffset(GetColumn(e)), _spearHeight/2),
          new Size(e.DesiredSize.Width, arrangeSize.Height - _spearHeight)));

      foreach (var e in _elements[(int)Pos.Row])
        e.Arrange(new Rect(
          new Point(_spearWidth/2, GetTopRowOffset(GetRow(e))),
          new Size(arrangeSize.Width - _spearWidth, e.DesiredSize.Height)));
    }
    private void ArrangeCells()
    {
      foreach (var e in _elements[(int)Pos.Cell])
        e.Arrange(new Rect(
          GetRightColumnOffset(GetColumn(e)-1),
          GetBottomRowOffset(GetRow(e)-1), 
          _cellSize, _cellSize));
    }

    #endregion

    #region ' Utils '

    private static Pos GetPos(int c, int r)
    {
      if (c == -1 && r == -1) return Pos.Background;

      if (c >= 1 && c <= 8 && r >= 1 && r <= 8) return Pos.Cell;

      if (c == 0 && (r >= 1 && r <= 8)) return Pos.LeftEdgeCell;
      if (c == 9 && (r >= 1 && r <= 8)) return Pos.RightEdgeCell;
      if (r == 0 && (c >= 1 && c <= 8)) return Pos.TopEdgeCell;
      if (r == 9 && (c >= 1 && c <= 8)) return Pos.BottomEdgeCell;

      if (c == 0 && r == 0) return Pos.BottomLeftCorner;
      if (c == 0 && r == 9) return Pos.TopLeftCorner;
      if (c == 9 && r == 0) return Pos.BottomRightCorner;
      if (c == 9 && r == 9) return Pos.TopRightCorner;

      if (c == -1 && (r >= 0 && r <= 8)) return Pos.Row;
      if (r == -1 && (c >= 0 && c <= 8)) return Pos.Column;

      return Pos.Unknown;
    }

    private T Measure<T>(Pos pos, Aggregate<T> agg, Func<UIElement, T> fetch)
    {
      return Measure(pos, InfiniteSize, agg, fetch);
    }
    private T Measure<T>(Pos pos, Size constraint, Aggregate<T> agg, Func<UIElement, T> fetch)
    {
      var collection = _elements[(int)pos];
      collection.ForEach(c => c.Measure(constraint));
      return agg(collection, fetch);
    }
    private static double Max(IEnumerable<UIElement> src, Func<UIElement, double> fetch)
    {
      return !src.Any() ? 0 : src.Max(fetch);
    }
    private static Size Max(IEnumerable<UIElement> src, Func<UIElement, Size> fetch)
    {
      return !src.Any()
               ? new Size(0, 0)
               : new Size(src.Max(c => fetch(c).Width),
                          src.Max(c => fetch(c).Height));
    }
    private static double Sum(IEnumerable<UIElement> src, Func<UIElement, double> fetch)
    {
      return !src.Any() ? 0 : src.Sum(fetch);
    }

    private double GetLeftBoardOffset()
    {
      return _spearWidth / 2 + _leftEdgeCellsWidth;
    }
    private double GetTopBoardOffset()
    {
      return _spearHeight / 2 + _topEdgeCellsHeight;
    }
    private double GetRightColumnOffset(int col)
    {
      return GetLeftBoardOffset() + _cellSize * col + _cols[col];
    }
    private double GetBottomRowOffset(int row)
    {
      return GetTopBoardOffset() + _cellSize * row + _rows[row];
    }
    private double GetLeftColumnOffset(int col)
    {
      if (col == 0) return GetLeftBoardOffset();
      return GetLeftBoardOffset() + _cellSize*col + _cols[col - 1];
    }
    private double GetTopRowOffset(int row)
    {
      if (row == 0) return GetTopBoardOffset();
      return GetTopBoardOffset() + _cellSize * row + _rows[row - 1];
    }

    #endregion
  }
}