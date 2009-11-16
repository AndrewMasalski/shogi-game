using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Yasc.GenericDragDrop;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Moves;
using Yasc.ShogiCore.Utils;
using Yasc.Utils;

namespace Yasc.Controls
{
  public class ShogiBoard : Control
  {
    private readonly Flag _dragMove = new Flag();
    private ShogiBoardCore BoardCore 
    {
      get { return this.FindChild<ShogiBoardCore>(); }
    }

    #region ' Helpers '

    static ShogiBoard()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ShogiBoard),
        new FrameworkPropertyMetadata(typeof(ShogiBoard)));
    }

    public ShogiBoard()
    {
      Dnd.AddDragHandler(this, OnDrag);
      Dnd.AddDropHandler(this, OnDrop);
    }

    #endregion

    #region ' Moves Animation '

    private void BoardOnMoving(object sender, MoveEventArgs args)
    {
      if (_dragMove) return;
      var m = args.Move as UsualMove;
      if (m == null) return;
      var from = RepresentedBoard[m.From.X, m.From.Y];
      var to = RepresentedBoard[m.To.X, m.To.Y];

      BoardCore.AnimateMove(from, to);
    }

    #endregion

    #region ' Drag'n'Drop Moves '

    private void OnDrop(object sender, DropEventArgs e)
    {
      ReleaseDragSource();
      if (e.DragTarget == null) return;

      var move = RecognizeMove(e);
      if (move == null) return;

      RaiseMoveAttemptEvent(move);

      if (move.IsValid)
        using (_dragMove.Set())
          RepresentedBoard.MakeMove(move);
    }
    private void OnDrag(object sender, RoutedEventArgs e)
    {
      var context = ((FrameworkElement)e.OriginalSource).DataContext;
      
      var cell = context as Cell;
      if (cell != null)
      {
        if (cell.Piece == null) 
          return; // Technically it's possible to drag empty cell

        var moves = RepresentedBoard.GetAvailableMoves(cell.Position);
        var positions = from UsualMove m in moves select m.To;
        BoardCore.HighlightAvailableMoves(positions.Distinct());
      }

      var piece = context as Piece;
      if (piece != null)
      {
        var moves = RepresentedBoard.GetAvailableMoves(piece);
        var positions = from DropMove m in moves select m.To;
        BoardCore.HighlightAvailableMoves(positions);
      }
    }

    private void ReleaseDragSource()
    {
      BoardCore.ResetAvailableMoves();
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
        return RepresentedBoard.GetDropMove(piece, to.Position);
      }
    }
    private MoveBase GetUsualMove(Cell from, Cell to)
    {
      MoveBase move;
      var m1 = RepresentedBoard.GetUsualMove(from.Position, to.Position, false);
      var m2 = RepresentedBoard.GetUsualMove(from.Position, to.Position, true);
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

    public static readonly DependencyProperty IsFlippedProperty =
      DependencyProperty.Register("IsFlipped", typeof (bool),
        typeof (ShogiBoard), new UIPropertyMetadata(false));

    public bool IsFlipped
    {
      get { return (bool) GetValue(IsFlippedProperty); }
      set { SetValue(IsFlippedProperty, value); }
    }

    public static readonly DependencyProperty RepresentedBoardProperty =
      DependencyProperty.Register("RepresentedBoard", typeof (Board),
                                  typeof (ShogiBoard), new UIPropertyMetadata(default(Board), PropertyChangedCallback));

    private static void PropertyChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs args)
    {
      if (args.OldValue != null)
        ((Board)args.OldValue).Moving -= ((ShogiBoard)o).BoardOnMoving;

      if (args.NewValue != null)
        ((Board)args.NewValue).Moving += ((ShogiBoard)o).BoardOnMoving;
    }

    public Board RepresentedBoard
    {
      get { return (Board) GetValue(RepresentedBoardProperty); }
      set { SetValue(RepresentedBoardProperty, value); }
    }
    
  }
}