using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Yasc.GenericDragDrop;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Moves;
using Yasc.Utils;

namespace Yasc.Controls
{
  [TemplatePart(Name = "PART_AdornerLayer", Type = typeof(Canvas))]
  public class ShogiBoard : Control
  {
    #region ' Ctors '

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
      if (_adornerLayer == null) return;
      if (_dragMove) return;
      
      var u = args.Move as UsualMove;
      if (u != null)
      {
        AnimateMove(GetCell(u.From), GetCell(u.To));
      }

      var d = args.Move as DropMove;
      if (d != null)
      {
        AnimateMove(GetNest(d.PieceType, d.Who.Color), GetCell(d.To));
      }
    }

    private void AnimateMove(PieceHolderBase fromControl, UIElement toCtrl)
    {
      var pieceControl = fromControl.ShogiPiece;
      MoveToAdornerLayer(fromControl);
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
      var duration = new Duration(TimeSpan.FromSeconds(.4));

      ctrl.BeginAnimation(Canvas.LeftProperty, new DoubleAnimation(to.X, duration));

      var anim = new DoubleAnimation(to.Y, duration);
      anim.Completed += completed;
      ctrl.BeginAnimation(Canvas.TopProperty, anim);
    }
    private void MoveToAdornerLayer(PieceHolderBase cell)
    {
      var piece = cell.ShogiPiece;
      var transform = piece.TransformToVisual(_adornerLayer);
      var point = transform.Transform(new Point(0, 0));
      Canvas.SetLeft(piece, point.X);
      Canvas.SetTop(piece, point.Y);
      piece.Width = piece.ActualWidth;
      piece.Height = piece.ActualHeight;
      _adornerLayer.Children.Add(cell.DeattachPiece());
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
      var moves = Board.GetAvailableMoves(args.PieceType, args.PieceColor);
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
          ArbitraryMove(fromHand.PieceType, fromHand.PieceColor, e.ToCell);
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
        ArbitraryMove(fromHand.PieceType, fromHand.PieceColor, e.ToHand);
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
        return Board.GetDropMove(fromHand.PieceType, e.ToPosition, Board[fromHand.PieceColor]);
      }
      throw new ArgumentOutOfRangeException("e");
    }

    private void ArbitraryMove(PieceType pieceType, PieceColor color, ShogiHand hand)
    {
      if (color == hand.Color) return;
      var piece = Board[color].GetPieceFromHandByType(pieceType);
      piece.Owner.Hand.Remove(piece);
      hand.Hand.Add(piece);
    }
    private static void ArbitraryMove(Cell cell, ShogiHand hand)
    {
      var piece = cell.Piece;
      cell.Piece = null;
      hand.Hand.Add(piece);
    }
    private void ArbitraryMove(PieceType pieceType, PieceColor color, Cell cell)
    {
      var piece = Board[color].GetPieceFromHandByType(pieceType);
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

    #region ' MoveSource Property '

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

    #endregion

    #region ' IsFlipped Property '

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

    #region ' Board Property '

    public static readonly DependencyProperty BoardProperty =
      DependencyProperty.Register("Board", typeof (Board),
       typeof (ShogiBoard), new UIPropertyMetadata(null, BoardPropertyChanged));

    private static void BoardPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
    {
      ((ShogiBoard)o).BoardPropertyChanged((Board)args.OldValue, (Board)args.NewValue);
    }

    private void BoardPropertyChanged(Board oldValue, Board newValue)
    {
      if (oldValue != null)
      {
        oldValue.Moving -= BoardOnMoving;
      }

      if (newValue != null)
      {
        newValue.Moving += BoardOnMoving;
      }
    }

    public Board Board
    {
      get { return (Board) GetValue(BoardProperty); }
      set { SetValue(BoardProperty, value); }
    }
    
    #endregion

    #region ' AreMoveRulesEnforced Property '

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

    #region ' Parts '

    public override void OnApplyTemplate()
    {
      _adornerLayer = GetTemplateChild("PART_AdornerLayer") as Canvas;
      base.OnApplyTemplate();
    }

    private Canvas _adornerLayer;

    #endregion

    #region ' Highlight Available Moves '

    private void HighlightAvailableMoves(IEnumerable<Position> cells)
    {
      foreach (Position p in cells)
      {
        var cell = GetCell(p);
        if (cell == null) continue;
        cell.IsPossibleMoveTarget = true;
      }
    }
    private void ResetAvailableMoves()
    {
      foreach (var p in Position.OnBoard)
      {
        var cell = GetCell(p);
        if (cell == null) continue;
        cell.IsPossibleMoveTarget = false;
      }
    }

    #endregion

    #region ' Helpers '

    public ShogiCell GetCell(Position position)
    {
      return Board == null ? null : GetCell(Board[position.X, position.Y]);
    }
    public ShogiCell GetCell(Cell cell)
    {
      return this.FindChild<ShogiCell>(c => c.Cell == cell);
    }
    public HandNest GetNest(PieceType type, PieceColor color)
    {
      var hand = this.FindChild<ShogiHand>(h => h.Color == color);
      return hand.FindLastChild<HandNest>(n => n.PieceType == type);
    }

    #endregion

    private readonly Dnd _dnd;
    private readonly Flag _dragMove = new Flag();
  }
}