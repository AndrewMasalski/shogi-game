using System;
using System.Windows;
using System.Windows.Controls;
using Yasc.ShogiCore.Core;

namespace Yasc.BoardControl.Common
{
  public class MovesAndCommentsGrid : Grid
  {
    public MovesAndCommentsGrid()
    {
      ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
      ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
    }

    protected override Size MeasureOverride(Size constraint)
    {
      _movesCount = 0;
      _currentRow = 0;
      _expectedRowsCount = 0;
      _lastWasComment = false;

      foreach (var child in InternalChildren)
        AddItem((ListBoxItem)child);

      ExpandGrid(Math.Max(0, _expectedRowsCount - RowDefinitions.Count));

      return base.MeasureOverride(constraint);
    }

    private void ExpandGrid(int rowsCount)
    {
      for (int i = 0; i < rowsCount; i++)
        RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
    }
    private int _movesCount;
    private int _currentRow;
    private int _expectedRowsCount;
    private bool _lastWasComment;

    public void AddItem(ListBoxItem element)
    {
      if (IsMove(element))
      {
        AddMove(element);
      }
      else
      {
        AddComment(element);
      }
    }

    private void AddMove(UIElement moveElement)
    {
      SetRow(moveElement, _currentRow);

      if (++_movesCount % 2 != 0)
      {
        SetColumn(moveElement, 0);
        _expectedRowsCount++;
      }
      else
      {
        SetColumn(moveElement, 1);
        _currentRow++;
      }
      _lastWasComment = false;
      SetColumnSpan(moveElement, 1);
      moveElement.Focusable = true;
    }

    private void AddComment(UIElement commentElement)
    {
      if (_movesCount % 2 == 1 && !_lastWasComment)
      {
        _currentRow++;
        _expectedRowsCount += 2;
      }
      else
      {
        _expectedRowsCount++;
      }
      SetRow(commentElement, _currentRow++);
      SetColumn(commentElement, 0);
      SetColumnSpan(commentElement, 2);
      commentElement.Focusable = false;
      _lastWasComment = true;
    }

    private static bool IsMove(ContentControl visualAdded)
    {
      return visualAdded.Content is DecoratedMove;
    }
  }
}
