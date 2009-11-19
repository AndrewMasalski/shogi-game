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
      _dnd = new Dnd(this);
      _dnd.DragFromBoard += OnDragFromBoard;
      _dnd.DragFromHand += OnDragFromHand;
      _dnd.DropToHand += OnDropToHand;
      _dnd.DropToBoard += OnDropToBoard;
      _dnd.DragCancelled += OnDragCancelled;
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

    private void OnDragFromBoard(object sender, DragFromBoardEventArgs args)
    {
      var moves = RepresentedBoard.GetAvailableMoves(args.FromPosition);
      var positions = from UsualMove m in moves select m.To;
      BoardCore.HighlightAvailableMoves(positions.Distinct());

      BoardCore.MoveSource = args.FromPosition;
    }
    private void OnDragFromHand(object sender, DragFromHandEventArgs args)
    {
      var moves = RepresentedBoard.GetAvailableMoves(args.Piece);
      var positions = from DropMove m in moves select m.To;
      BoardCore.HighlightAvailableMoves(positions);
    }

    private void OnDropToBoard(object sender, DropToBoardEventArgs e)
    {
      ReleaseDragSource();

      if (AreMoveRulesEnforced)
      {
        var move = RecognizeMove(e);
        if (move == null) return;

        RaiseMoveAttemptEvent(move);

        if (move.IsValid)
          using (_dragMove.Set())
            RepresentedBoard.MakeMove(move);
      }
      else
      {
        var fromBoard = e.From as DragFromBoardEventArgs;
        if (fromBoard != null)
        {
          M(fromBoard.FromCell, e.ToCell);
        }
        var fromHand = e.From as DragFromHandEventArgs;
        if (fromHand != null)
        {
          M(fromHand.Piece, e.ToCell);
        }
      }
    }
    private void OnDropToHand(object sender, DropToHandEventArgs args)
    {
      ReleaseDragSource();
    }
    private void OnDragCancelled(object sender, DragFromEventArgs args)
    {
      ReleaseDragSource();
    }

    private MoveBase RecognizeMove(DropToBoardEventArgs e)
    {
      var fromBoard = e.From as DragFromBoardEventArgs;
      if (fromBoard != null)
      {
        return GetUsualMove(fromBoard.FromCell, e.ToCell);
      }
      var fromHand = e.From as DragFromHandEventArgs;
      if (fromHand != null)
      {
        return RepresentedBoard.GetDropMove(fromHand.Piece, e.ToPosition);
      }
      throw new ArgumentOutOfRangeException("e");
    }

    private void M(Piece piece, Piece o)
    {
      throw new NotImplementedException();
    }

    private void M(Cell piece, Piece o)
    {
      throw new NotImplementedException();
    }

    private void M(Piece piece, Cell cell)
    {
      piece.Owner.Hand.Remove(piece);
      var m = RepresentedBoard[cell.Position];
      RepresentedBoard[cell.Position] = null;
      RepresentedBoard[cell.Position] = piece;
      if (m != null) piece.Owner.Hand.Add(m);
    }

    private void M(Cell from, Cell to)
    {
      var piece = RepresentedBoard[from.Position];
      RepresentedBoard[from.Position] = null;
      var m = RepresentedBoard[to.Position];
      RepresentedBoard[to.Position] = null;
      RepresentedBoard[to.Position] = piece;
      if (m != null) piece.Owner.Hand.Add(m);
    }



    private void ReleaseDragSource()
    {
      BoardCore.ResetAvailableMoves();
      BoardCore.MoveSource = null;
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
      DependencyProperty.RegisterAttached("IsFlipped", typeof (bool),
        typeof (ShogiBoard), new FrameworkPropertyMetadata(false, 
          FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public bool IsFlipped
    {
      get { return (bool) GetValue(IsFlippedProperty); }
      set { SetValue(IsFlippedProperty, value); }
    }

    public static bool GetIsFlipped(DependencyObject obj)
    {
      return (bool)obj.GetValue(IsFlippedProperty);
    }
    public static void SetIsFlipped(DependencyObject obj, bool value)
    {
      obj.SetValue(IsFlippedProperty, value);
    }

    public static readonly DependencyProperty RepresentedBoardProperty =
      DependencyProperty.Register("RepresentedBoard", typeof (Board),
                                  typeof (ShogiBoard), new UIPropertyMetadata(default(Board), RepresentedBoardPropertyChanged));

    private static void RepresentedBoardPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
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

    public static readonly DependencyProperty AreMoveRulesEnforcedProperty =
      DependencyProperty.Register("AreMoveRulesEnforced", typeof (bool),
                                  typeof (ShogiBoard), new UIPropertyMetadata(true, PropertyChangedCallback));

    private readonly Dnd _dnd;

    private static void PropertyChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs args)
    {
      ((ShogiBoard)o).Prop((bool) args.NewValue);
    }

    private void Prop(bool value)
    {
      RepresentedBoard.IsMovesOrderMaintained = value;
    }

    public bool AreMoveRulesEnforced
    {
      get { return (bool) GetValue(AreMoveRulesEnforcedProperty); }
      set { SetValue(AreMoveRulesEnforcedProperty, value); }
    }
    
    
  }
}