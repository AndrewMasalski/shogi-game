using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Yasc.GenericDragDrop;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Utils;

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
      get { return (Board)DataContext; }
    }

    private ShogiCell GetCell(Cell cell)
    {
      return _cells.ItemContainerGenerator.
        ContainerFromItem(cell).FindChild<ShogiCell>();
    }
    private ShogiCell GetCell(Position p)
    {
      return GetCell(Board[p.X, p.Y]);
    }

    #endregion

    #region ' Public Interface '

    public void AnimateMove(Cell from, Cell to)
    {
      var generator = _cells.ItemContainerGenerator;
      AnimateMove(generator.ContainerFromItem(from),
                  (FrameworkElement)generator.ContainerFromItem(to));
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
      ctrl.BeginAnimation(Canvas.LeftProperty,
         new DoubleAnimation(to.X, new Duration(TimeSpan.FromSeconds(.25))));

      var anim = new DoubleAnimation(to.Y, new Duration(TimeSpan.FromSeconds(.25)));
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
      RemoveFromParentControl(ctrl);
      _adornerLayer.Children.Add(ctrl);
    }
    private static void RemoveFromParentControl(FrameworkElement ctrl)
    {
      ((Grid)ctrl.Parent).Children.Remove(ctrl);
    }

    #endregion
  }
}