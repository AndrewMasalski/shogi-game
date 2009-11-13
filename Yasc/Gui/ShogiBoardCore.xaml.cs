using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Yasc.GenericDragDrop;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Moves;
using Yasc.ShogiCore.Utils;

namespace Yasc.Gui
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

    #endregion

    #region ' Move Animation '

    private ShogiPiece GetPiece(Cell cell)
    {
      return _cells.ItemContainerGenerator.
        ContainerFromItem(cell).FindChild<ShogiPiece>();
    }
    private ShogiPiece GetPiece(Position p)
    {
      return GetPiece(Board[p.X, p.Y]);
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

    public void AnimateMove(Cell from, Cell to)
    {
      var generator = _cells.ItemContainerGenerator;
      AnimateMove(generator.ContainerFromItem(from),
         (FrameworkElement)generator.ContainerFromItem(to));
    }

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

    public void HighlightAvailableMoves(IEnumerable<MoveBase> moves)
    {
      foreach (UsualMove move in moves)
        GetCell(move.To).IsPossibleMoveTarget = true;
    }

    public void ResetAvailableMoves()
    {
      foreach (var p in Position.OnBoard)
        GetCell(p).IsPossibleMoveTarget = false;
    }
  }
}