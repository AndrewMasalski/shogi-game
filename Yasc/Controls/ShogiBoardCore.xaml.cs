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
  public partial class ShogiBoardCore
  {
    #region ' Helpers '

    public ShogiBoardCore()
    {
      InitializeComponent();
    }

    public Board Board
    {
      get { return this.FindAncestor<ShogiBoard>().RepresentedBoard; }
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
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
      if (e.Property == ShogiBoard.IsFlippedProperty)
      {
        RefillCells((bool) e.NewValue);
      }
      base.OnPropertyChanged(e);
    }
    private void RefillCells(bool isFlipped)
    {
      _cells.ItemsSource = !isFlipped ? Board : Flip(Board);
    }

    private static IEnumerable<Cell> Flip(IEnumerable<Cell> board)
    {
      var list = board.ToList();
      for (int i = 8; i >= 0; i--)
        for (int j = 8; j >= 0; j--)
          yield return list[i*9 + j];
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      RefillCells(ShogiBoard.GetIsFlipped(this));
    }
  }
}