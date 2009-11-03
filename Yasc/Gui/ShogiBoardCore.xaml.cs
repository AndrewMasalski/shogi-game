using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Yasc.GenericDragDrop;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Moves;
using Yasc.ShogiCore.Utils;
using Yasc.Utils;

namespace Yasc.Gui
{
  public partial class ShogiBoardCore
  {
    public ShogiBoardCore()
    {
      InitializeComponent();
      DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs args)
    {
      Board.Move += BoardOnMove;
    }

    private void BoardOnMove(object sender, MoveEventArgs args)
    {
      if (_dragMove) return;
      var m = args.Move as UsualMove;
      if (m == null) return;
      var from = Board[m.From.X, m.From.Y];
      var to = Board[m.To.X, m.To.Y];
      var fromCtrl = (FrameworkElement)_cells.ItemContainerGenerator.ContainerFromItem(from);
      var toCtrl = (FrameworkElement)_cells.ItemContainerGenerator.ContainerFromItem(to);
      
      var pieceControl = fromCtrl.FindChild<ShogiPiece>();
      var renderSize = pieceControl.RenderSize;
      var fromTransform = (MatrixTransform)fromCtrl.TransformToVisual(_adornerLayer).Clone();
      var toTransform = (MatrixTransform)toCtrl.TransformToVisual(_adornerLayer);
//      _adornerLayer.RenderTransform = fromTransform;
      ((Grid)pieceControl.Parent).Children.Remove(pieceControl);
      pieceControl.Width = renderSize.Width;
      pieceControl.Height = renderSize.Height;
      toCtrl.Visibility = Visibility.Hidden;
      _adornerLayer.Children.Add(pieceControl);

//      new DispatcherTimer(TimeSpan.FromSeconds(5),
//           DispatcherPriority.ApplicationIdle, (o, eventArgs) =>
//              {
//                fromCtrl.Visibility = Visibility.Visible;
//                toCtrl.Visibility = Visibility.Visible;
//                _adornerLayer.Children.Remove(pieceControl);
//              }, Dispatcher);
      var animation = new MatrixAnimationUsingKeyFrames();
      animation.KeyFrames.Add(new DiscreteMatrixKeyFrame(fromTransform.Matrix, KeyTime.FromTimeSpan(TimeSpan.Zero)));
      animation.KeyFrames.Add(new DiscreteMatrixKeyFrame(toTransform.Matrix, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1))));
      animation.Duration = Duration.Automatic;
      Storyboard.SetTarget(animation, fromTransform);
      Storyboard.SetTargetProperty(animation, new PropertyPath("Matrix"));
      int counter = 0;
      animation.Completed += (o, eventArgs) => counter++;
      var sb = new Storyboard();
      sb.Children.Add(animation);
//      sb.Duration = TimeSpan.FromSeconds(4);
      sb.Begin();
    }

    public Board Board
    {
      get { return (Board)DataContext; }
    }

    #region ' MoveAttempt '

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

    private readonly Flag _dragMove = new Flag();

    private void OnDragDrop(object sender, DropEventArgs e)
    {
      MoveBase move;
      if (e.DragSource.DataContext is Cell)
      {
        var from = (Cell)e.DragSource.DataContext;
        var to = (Cell)e.DragTarget.DataContext;
        move = GetUsualMove(from, to);
      }
      else
      {
        var piece = (Piece)e.DragSource.DataContext;
        var to = (Cell)e.DragTarget.DataContext;
        move = Board.GetDropMove(piece, to.Position);
      }

      if (move == null) return;
      RaiseMoveAttemptEvent(move);

      if (move.IsValid)
        using (_dragMove.Set())
          Board.MakeMove(move);
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
  }
}