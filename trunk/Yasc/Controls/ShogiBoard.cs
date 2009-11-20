using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Yasc.GenericDragDrop;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Moves;
using Yasc.ShogiCore.Utils;
using Yasc.Utils;

namespace Yasc.Controls
{
  [TemplatePart(Name = "PART_AdornerLayer", Type = typeof(Canvas))]
  public class ShogiBoard : Control
  {
    private readonly Flag _dragMove = new Flag();

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
      var from = Board[m.From.X, m.From.Y];
      var to = Board[m.To.X, m.To.Y];

      AnimateMove(from, to);
    }

    #endregion

    #region ' Drag'n'Drop Moves '

    private void OnDragFromBoard(object sender, DragFromBoardEventArgs args)
    {
      var moves = Board.GetAvailableMoves(args.FromPosition);
      var positions = from UsualMove m in moves select m.To;
      HighlightAvailableMoves(positions.Distinct());

      MoveSource = args.FromPosition;
    }
    private void OnDragFromHand(object sender, DragFromHandEventArgs args)
    {
      var moves = Board.GetAvailableMoves(args.Piece);
      var positions = from DropMove m in moves select m.To;
      HighlightAvailableMoves(positions);
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
            Board.MakeMove(move);
      }
      else
      {
        var fromBoard = e.From as DragFromBoardEventArgs;
        if (fromBoard != null)
        {
          ArbitraryMove(fromBoard.FromCell, e.ToCell);
        }
        var fromHand = e.From as DragFromHandEventArgs;
        if (fromHand != null)
        {
          ArbitraryMove(fromHand.Piece, e.ToCell);
        }
      }
    }
    private void OnDropToHand(object sender, DropToHandEventArgs e)
    {
      ReleaseDragSource();
      if (AreMoveRulesEnforced) return;

      var fromBoard = e.From as DragFromBoardEventArgs;
      if (fromBoard != null)
      {
        ArbitraryMove(fromBoard.FromCell, e.ToHand);
      }
      var fromHand = e.From as DragFromHandEventArgs;
      if (fromHand != null)
      {
        ArbitraryMove(fromHand.Piece, e.ToHand);
      }
      
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
        return Board.GetDropMove(fromHand.Piece, e.ToPosition);
      }
      throw new ArgumentOutOfRangeException("e");
    }

    private static void ArbitraryMove(Piece piece, ShogiHand hand)
    {
      if (piece.Color == hand.Color) return;
      piece.Owner.Hand.Remove(piece);
      hand.Hand.Add(piece);
    }
    private static void ArbitraryMove(Cell cell, ShogiHand hand)
    {
      var piece = cell.Piece;
      cell.Piece = null;
      hand.Hand.Add(piece);
    }
    private static void ArbitraryMove(Piece piece, Cell cell)
    {
      piece.Owner.Hand.Remove(piece);
      var m = cell.Piece;
      cell.Piece = null;
      cell.Piece = piece;
      if (m != null) piece.Owner.Hand.Add(m);
    }
    private static void ArbitraryMove(Cell from, Cell to)
    {
      var piece = from.Piece;
      from.Piece = null;
      var m = to.Piece;
      to.Piece = null;
      to.Piece = piece;
      if (m != null) piece.Owner.Hand.Add(m);
    }

    private void ReleaseDragSource()
    {
      ResetAvailableMoves();
      MoveSource = null;
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

    #region IsFlippedProperty

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


    #endregion

    #region BoardProperty

    public static readonly DependencyProperty BoardProperty =
      DependencyProperty.Register("Board", typeof (Board),
                                  typeof (ShogiBoard), new UIPropertyMetadata(default(Board), BoardPropertyChanged));

    private static void BoardPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
    {
      if (args.OldValue != null)
        ((Board)args.OldValue).Moving -= ((ShogiBoard)o).BoardOnMoving;

      if (args.NewValue != null)
        ((Board)args.NewValue).Moving += ((ShogiBoard)o).BoardOnMoving;
    }

    public Board Board
    {
      get { return (Board) GetValue(BoardProperty); }
      set { SetValue(BoardProperty, value); }
    }


    #endregion

    #region AreMoveRulesEnforcedProperty

    public static readonly DependencyProperty AreMoveRulesEnforcedProperty =
      DependencyProperty.Register("AreMoveRulesEnforced", typeof (bool),
                                  typeof (ShogiBoard), new UIPropertyMetadata(true, OnMoveRulesEnforcedChanged));


    private static void OnMoveRulesEnforcedChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
    {
      ((ShogiBoard)o).OnMoveRulesEnforcedChanged((bool) args.NewValue);
    }

    private void OnMoveRulesEnforcedChanged(bool value)
    {
      Board.IsMovesOrderMaintained = value;
    }

    public bool AreMoveRulesEnforced
    {
      get { return (bool) GetValue(AreMoveRulesEnforcedProperty); }
      set { SetValue(AreMoveRulesEnforcedProperty, value); }
    }

    #endregion

    private readonly Dnd _dnd;

    private Canvas _adornerLayer;

    public override void OnApplyTemplate()
    {
      _adornerLayer = GetTemplateChild("PART_AdornerLayer") as Canvas;
      base.OnApplyTemplate();
    }
    
    #region ' Public Interface '

    public void AnimateMove(Cell from, Cell to)
    {
      if (_adornerLayer == null) return;
      AnimateMove(GetCell(from), GetCell(to));
    }
    public void HighlightAvailableMoves(IEnumerable<Position> cells)
    {
      foreach (Position p in cells)
      {
        var cell = GetCell(p);
        if (cell == null) continue;
        cell.IsPossibleMoveTarget = true;
      }
    }
    public void ResetAvailableMoves()
    {
      foreach (var p in Position.OnBoard)
      {
        var cell = GetCell(p);
        if (cell == null) continue;
        cell.IsPossibleMoveTarget = false;
      }
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
          GetCell((Position)_moveSource).IsMoveSource = false;
        }
        _moveSource = value;
        if (_moveSource != null)
        {
          GetCell((Position)_moveSource).IsMoveSource = true;
        }
      }
    }

    private ShogiCell GetCell(Cell cell)
    {
      return this.FindChild<ShogiCell>(c => c.Cell == cell);
    }
    private ShogiCell GetCell(Position p)
    {
      return Board == null ? null : GetCell(Board[p.X, p.Y]);
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
  }
}