using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Yasc.GenericDragDrop;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Utils;
using System.Linq;

namespace Yasc.Controls
{
  [TemplatePart(Name = "PART_AdornerLayer", Type = typeof(Canvas))]
  public class ShogiBoardCore : Control
  {
    static ShogiBoardCore()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ShogiBoardCore),
        new FrameworkPropertyMetadata(typeof(ShogiBoardCore)));
    }

    public override void OnApplyTemplate()
    {
      _adornerLayer = GetTemplateChild("PART_AdornerLayer") as Canvas;
      base.OnApplyTemplate();
    }
    #region IsFlippedProperty

    public static readonly DependencyProperty IsFlippedProperty = ShogiBoard.IsFlippedProperty.AddOwner(
      typeof(ShogiBoardCore), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, OnIsFlippedChanged));

    private static void OnIsFlippedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {

      ((ShogiBoardCore)d).OnIsFlippedChanged((bool)e.NewValue);
    }

    private void OnIsFlippedChanged(bool value)
    {
      Cells = !value ? Board : Flip(Board);
    }

    public bool IsFlipped
    {
      get { return (bool)GetValue(IsFlippedProperty); }
      set { SetValue(IsFlippedProperty, value); }
    }

    #endregion

    #region ' Helpers '

    public static readonly DependencyProperty BoardProperty =
      DependencyProperty.Register("Board", typeof (Board),
        typeof (ShogiBoardCore), new UIPropertyMetadata(null, OnBoardChanged));

    private static void OnBoardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ShogiBoardCore) d).OnBoardChanged((Board) e.NewValue);
    }

    private void OnBoardChanged(IEnumerable<Cell> board)
    {
      Cells = !IsFlipped ? board : Flip(board);
    }

    public Board Board
    {
      get { return (Board) GetValue(BoardProperty); }
      set { SetValue(BoardProperty, value); }
    }


    public static readonly DependencyProperty CellsProperty =
      DependencyProperty.Register("Cells", typeof(IEnumerable<Cell>),
        typeof (ShogiBoardCore), new UIPropertyMetadata(null));

    public IEnumerable<Cell> Cells
    {
      get { return (IEnumerable<Cell>) GetValue(CellsProperty); }
      set { SetValue(CellsProperty, value); }
    }
    
    private ShogiCell GetCell(Cell cell)
    {
      return this.FindChild<ShogiCell>(c => c.Cell == cell);
    }
    private ShogiCell GetCell(Position p)
    {
      return GetCell(Board[p.X, p.Y]);
    }

    #endregion

    #region ' Public Interface '

    public void AnimateMove(Cell from, Cell to)
    {
      if (_adornerLayer == null) return;
      AnimateMove(GetCell(from), GetCell(to));
    }
    public void HighlightAvailableMoves(IEnumerable<Position> cells)
    {
      foreach (Position p in cells)
        GetCell(p).IsPossibleMoveTarget = true;
    }
    public void ResetAvailableMoves()
    {
      foreach (var p in Position.OnBoard)
        GetCell(p).IsPossibleMoveTarget = false;
    }

    private Position? _moveSource;
    private Canvas _adornerLayer;

    public Position? MoveSource
    {
      get { return _moveSource; }
      set
      {
        if (_moveSource == value) return;
        
        if (_moveSource != null)
        {
          GetCell((Position) _moveSource).IsMoveSource = false;
        }
        _moveSource = value;
        if (_moveSource != null)
        {
          GetCell((Position)_moveSource).IsMoveSource = true;
        }
      }
    }

    #endregion

    #region ' Move Animation '

    private void AnimateMove(DependencyObject fromCtrl, UIElement toCtrl)
    {
      var pieceControl = fromCtrl.FindChild<ShogiPiece>();
      MoveToAdornerLayer(pieceControl);
      var to = toCtrl.TransformToVisual(_adornerLayer).Transform(new Point(0, 0));
      toCtrl.Visibility = Visibility.Hidden;

      AnimatePosition(pieceControl, to, (sender, args) =>
          {
            toCtrl.Visibility = Visibility.Visible;
            _adornerLayer.Children.Remove(pieceControl);
          });
    }
    private static void AnimatePosition(IAnimatable ctrl, Point to, EventHandler completed)
    {
      var duration = new Duration(TimeSpan.FromSeconds(.3));
      
      ctrl.BeginAnimation(Canvas.LeftProperty, new DoubleAnimation(to.X, duration));

      var anim = new DoubleAnimation(to.Y, duration);
      anim.Completed += completed;
      ctrl.BeginAnimation(Canvas.TopProperty, anim);
    }
    private void MoveToAdornerLayer(FrameworkElement ctrl)
    {
      var transform = ctrl.TransformToVisual(_adornerLayer);
      var point = transform.Transform(new Point(0, 0));
      Canvas.SetLeft(ctrl, point.X);
      Canvas.SetTop(ctrl, point.Y);
      ctrl.Width = ctrl.ActualWidth;
      ctrl.Height = ctrl.ActualHeight;
      // When we move piece from its tamplate piece DataContext changes 
      // to the one _adornelLyer has. There's nothing wrong with it except
      // XAML might define bindings which become invalid. As far as piece
      // is going to be thrown away after animation it's not a big deal.
      ctrl.DataContext = ctrl.DataContext;
      RemoveFromParentControl(ctrl);
      _adornerLayer.Children.Add(ctrl);
    }
    private static void RemoveFromParentControl(DependencyObject ctrl)
    {
      ((ContentPresenter)VisualTreeHelper.GetParent(ctrl)).Content = null;
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
  }
}