using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Yasc.GenericDragDrop;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Moves;
using Yasc.ShogiCore.Utils;
using Yasc.Utils;

namespace Yasc.Gui
{
  public class ShogiBoard : Control
  {
    private readonly Flag _dragMove = new Flag();
    private ShogiBoardCore _boardCore;

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
      DataContextChanged += ((sender, args) => {Board.Move += BoardOnMove;});
    }

    public Board Board
    {
      get { return (Board)DataContext; }
    }

    public override void OnApplyTemplate()
    {
      _boardCore = this.FindChild<ShogiBoardCore>();
      base.OnApplyTemplate();
    }

    #endregion

    #region ' Moves Animation '

    private void BoardOnMove(object sender, MoveEventArgs args)
    {
      if (_dragMove) return;
      var m = args.Move as UsualMove;
      if (m == null) return;
      var from = Board[m.From.X, m.From.Y];
      var to = Board[m.To.X, m.To.Y];

      _boardCore.AnimateMove(from, to);
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
          Board.MakeMove(move);
    }
    private void OnDrag(object sender, RoutedEventArgs e)
    {
      var context = ((FrameworkElement)e.OriginalSource).DataContext;
      
      var cell = context as Cell;
      if (cell != null)
      {
        if (cell.Piece == null) 
          return; // Technically it's possible to drag empty cell

        var moves = Board.GetAvailableMoves(cell.Position);
        var positions = from UsualMove m in moves select m.To;
        _boardCore.HighlightAvailableMoves(positions.Distinct());
      }

      var piece = context as Piece;
      if (piece != null)
      {
        // Need to highlight available drop moves
      }
    }

    private void ReleaseDragSource()
    {
      _boardCore.ResetAvailableMoves();
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
  }
}