﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Yasc.GenericDragDrop;
using Yasc.ShogiCore;
using System.Linq;

namespace Yasc.Controls
{
  public class ShogiBoardCore : Control
  {
    static ShogiBoardCore()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ShogiBoardCore),
        new FrameworkPropertyMetadata(typeof(ShogiBoardCore)));
    }

    #region ' IsFlipped Property '

    public static readonly DependencyProperty IsFlippedProperty = ShogiBoard.IsFlippedProperty.AddOwner(
      typeof(ShogiBoardCore), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, OnIsFlippedChanged));

    private static void OnIsFlippedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {

      ((ShogiBoardCore)d).OnIsFlippedChanged((bool)e.NewValue);
    }

    private void OnIsFlippedChanged(bool value)
    {
      if (Board == null)
      {
        Cells = null;
      }
      else
      {
        Cells = !value ? Board.Cells : Flip(Board.Cells);
      }
    }

    public bool IsFlipped
    {
      get { return (bool)GetValue(IsFlippedProperty); }
      set { SetValue(IsFlippedProperty, value); }
    }

    #endregion

    #region ' Board Property '

    public static readonly DependencyProperty BoardProperty =
      DependencyProperty.Register("Board", typeof (Board),
                  typeof (ShogiBoardCore), new UIPropertyMetadata(null, OnBoardChanged));

    private static void OnBoardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ShogiBoardCore) d).OnBoardChanged((Board) e.NewValue);
    }

    private void OnBoardChanged(Board board)
    {
      if (board == null)
      {
        Cells = null;
      }
      else
      {
        Cells = !IsFlipped ? board.Cells : Flip(board.Cells);
      }
    }

    public Board Board
    {
      get { return (Board) GetValue(BoardProperty); }
      set { SetValue(BoardProperty, value); }
    }

    #endregion

    #region ' Cells Property '

    public static readonly DependencyProperty CellsProperty =
      DependencyProperty.Register("Cells", typeof(IEnumerable<Cell>),
        typeof (ShogiBoardCore), 
        new UIPropertyMetadata(null, OnCellsPropertyChanged));

    private static void OnCellsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ShogiBoardCore) d)._shogiCells = null;
    }

    public IEnumerable<Cell> Cells
    {
      get { return (IEnumerable<Cell>) GetValue(CellsProperty); }
      set { SetValue(CellsProperty, value); }
    }
    
    #endregion

    private static IEnumerable<Cell> Flip(IEnumerable<Cell> board)
    {
      if (board == null) yield break;
      var list = board.ToList();
      for (int i = 8; i >= 0; i--)
        for (int j = 8; j >= 0; j--)
          yield return list[i*9 + j];
    }

    public override void OnApplyTemplate()
    {
      _shogiCells = null;
      base.OnApplyTemplate();
    }
    public ShogiCell GetCell(Position position)
    {
      return ShogiCells[position.X, position.Y];
    }
    private ShogiCell[,] _shogiCells;
    public ShogiCell[,] ShogiCells
    {
      get
      {
        if (_shogiCells == null)
        {
          _shogiCells = new ShogiCell[9, 9];
          foreach (var cell in this.FindChildren<ShogiCell>())
          {
            var position = cell.Cell.Position;
            _shogiCells[position.X, position.Y] = cell;
          }
        }
        return _shogiCells;
      }
    }
  }
}