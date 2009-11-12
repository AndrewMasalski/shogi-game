using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Yasc.GenericDragDrop;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Moves;
using Yasc.ShogiCore.Utils;
using Yasc.Utils;

namespace Yasc.Gui
{
  public partial class ShogiBoardCore
  {
    private readonly Flag _dragMove = new Flag();

    #region ' Helpers '

    public ShogiBoardCore()
    {
      InitializeComponent();
      DataContextChanged += OnDataContextChanged;
    }

    public Board Board
    {
      get { return (Board)DataContext; }
    }
    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs args)
    {
      Board.Move += BoardOnMove;
    }

    #endregion

    #region ' Move Animation '

    private void BoardOnMove(object sender, MoveEventArgs args)
    {
      if (_dragMove) return;
      var m = args.Move as UsualMove;
      if (m == null) return;
      var from = Board[m.From.X, m.From.Y];
      var to = Board[m.To.X, m.To.Y];

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

    #region ' MoveAttempt Routed Event '

    public static readonly RoutedEvent MoveAttemptEvent = EventManager.
      RegisterRoutedEvent("MoveAttempt", RoutingStrategy.Bubble,
        typeof(EventHandler<MoveAttemptEventArgs>), typeof(ShogiBoardCore));
    public event EventHandler<MoveAttemptEventArgs> MoveAttempt
    {
      add { AddHandler(MoveAttemptEvent, value); }
      remove { RemoveHandler(MoveAttemptEvent, value); }
    }
    private void RaiseMoveAttemptEvent(MoveBase move)
    {
      RaiseEvent(new MoveAttemptEventArgs(MoveAttemptEvent, this, move));
    }

    #endregion

    #region ' Drag'n'Drop Moves '

    private void OnDragDrop(object sender, DropEventArgs e)
    {
      var move = RecognizeMove(e);
      if (move == null) return;

      RaiseMoveAttemptEvent(move);

      if (move.IsValid)
        using (_dragMove.Set())
          Board.MakeMove(move);
    }
    private MoveBase RecognizeMove(DropEventArgs e)
    {
      if (e.DragSource.DataContext is Cell)
      {
        var from = (Cell)e.DragSource.DataContext;
        var to = (Cell)e.DragTarget.DataContext;
        return GetUsualMove(from, to);
      }
      else
      {
        var piece = (Piece)e.DragSource.DataContext;
        var to = (Cell)e.DragTarget.DataContext;
        return Board.GetDropMove(piece, to.Position);
      }
    }
    private MoveBase GetUsualMove(Cell from, Cell to)
    {
      MoveBase move;
      var m1 = Board.GetUsualMove(from.Position, to.Position, false);
      var m2 = Board.GetUsualMove(from.Position, to.Position, true);
      if (m1.IsValid && m2.IsValid)
      {
        var answer = MessageBox.Show("Promote?", "Q",
                                     MessageBoxButton.YesNo, MessageBoxImage.Question);
        move = answer == MessageBoxResult.Yes ? m2 : m1;
      }
      else move = m1.IsValid ? m1 : m2;
      return move;
    }

    #endregion
  }
}