using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Yasc.BoardControl.Controls.Automation;
using Yasc.BoardControl.GenericDragDrop;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Moves;
using Yasc.Utils;

namespace Yasc.BoardControl.Controls
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

    public ShogiBoardCore Core { get; private set; }
    private readonly List<ShogiHand> _hands = new List<ShogiHand>(2);
    public ShogiHand WhiteHand
    {
      get { return _hands[0].Color == PieceColor.White ? _hands[0] : _hands[1]; }
    }
    public ShogiHand BlackHand
    {
      get { return _hands[0].Color == PieceColor.Black ? _hands[0] : _hands[1]; }
    }

    internal bool AreChildrenLoaded
    {
      get { return Core != null && _hands.Count == 2; }
    }

    #endregion

    #region ' Moves Animation '

    private void BoardOnMoving(object sender, MoveEventArgs args)
    {
      if (_dragMove) return;
      AnimateMove(args.Move);
    }
    private void AnimateMove(MoveBase move)
    {
      if (_adornerLayer == null) return;
      var u = move as UsualMove;
      if (u != null)
      {
        AnimateMove(GetCell(u.From), GetCell(u.To));
      }

      var d = move as DropMove;
      if (d != null)
      {
        AnimateMove(this[d.Who.Color][d.PieceType], GetCell(d.To));
      }
    }
    private void AnimateInvertedMove(MoveBase move)
    {
      if (_adornerLayer == null) return;
      var u = move as UsualMove;
      if (u != null)
      {
        AnimateMove(GetCell(u.To), GetCell(u.From));
      }

      var d = move as DropMove;
      if (d != null)
      {
        var hand = this[d.Who.Color];
        AnimateMove(GetCell(d.To), hand[d.PieceType] ?? (UIElement)hand);
      }
    }

    public void TraceConnectivity()
    {
      int counter = 0, disc = 0;
      foreach (var p in Position.OnBoard)
      {
        var cell = GetCell(p);
        if (cell.ShogiPiece != null)
        {
          counter++;
          if (cell.ShogiPiece.FindCommonVisualAncestor(cell) == null)
//          if (cell.ShogiPiece.FindCommonVisualAncestor(_adornerLayer) == null)
          {
            disc++;
          }
        }
      }
      Console.WriteLine("{0} bad of {1}", disc, counter);
    }

    private void AnimateMove(PieceHolderBase fromControl, UIElement toCtrl)
    {
      var pieceControl = fromControl.ShogiPiece;
      // TODO: Find out what's going on here
      if (pieceControl.FindCommonVisualAncestor(_adornerLayer) == null) return;
      MoveToAdornerLayer(fromControl);
      var to = toCtrl.TransformToVisual(_adornerLayer).Transform(new Point(0, 0));
      toCtrl.Visibility = Visibility.Hidden;

      AnimatePosition(pieceControl, to, (sender, args) =>
      {
        // NOTE: Could it be that we start animation twice on the same piece and one "remove" don't come?
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
      _adornerLayer.Children.Add(cell.DetachPiece());
    }

    #endregion

    #region ' Drag'n'Drop Moves '

    private void OnDragFromBoard(object sender, DragFromBoardEventArgs args)
    {
      var moves = Board.GetAvailableMoves(args.FromPosition);
      var positions = from m in moves select m.To;

      foreach (var p in positions.Distinct())
        GetCell(p).IsPossibleMoveTarget = true;

      var promoting = from m in moves where m.IsPromoting select m.To;

      foreach (var p in promoting)
        GetCell(p).IsPromotionAllowed = true;

      if (args.Piece.PieceType != "銀")
        foreach (var p in promoting)
          GetCell(p).IsPromotionRecommended = true;

      MoveSource = args.FromPosition;
    }
    private void OnDragFromHand(object sender, DragFromHandEventArgs args)
    {
      var moves = Board.GetAvailableMoves(args.PieceType, args.PieceColor);
      foreach (var p in from m in moves select m.To)
        GetCell(p).IsPossibleMoveTarget = true;
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
          ArbitraryMove(fromBoard.FromCell, e.ToCell, e.PromotionRequest);
        }
        var fromHand = e.From as DragFromHandEventArgs;
        if (fromHand != null)
        {
          ArbitraryMove(fromHand.PieceType, fromHand.PieceColor, e.ToCell, e.PromotionRequest);
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
        return Board.GetUsualMove(fromBoard.FromCell.Position, e.ToCell.Position, e.PromotionRequest);
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
      cell.ResetPiece();
      hand.Hand.Add(piece);
    }
    private void ArbitraryMove(PieceType pieceType, PieceColor color, Cell cell, bool promotionRequest)
    {
      var piece = Board[color].GetPieceFromHandByType(pieceType);
      var owner = piece.Owner;
      if (promotionRequest) piece.IsPromoted = true;
      piece.Owner.Hand.Remove(piece);
      var m = cell.Piece;
      cell.ResetPiece();
      cell.SetPiece(piece, owner);
      if (m != null) piece.Owner.Hand.Add(m);
    }
    private static void ArbitraryMove(Cell from, Cell to, bool promotionRequest)
    {
      var piece = from.Piece;
      var owner = piece.Owner;
      if (promotionRequest) piece.IsPromoted = true;
      from.ResetPiece();
      var m = to.Piece;
      to.ResetPiece();
      to.SetPiece(piece, owner);
      if (m != null) piece.Owner.Hand.Add(m);
    }

    private void ReleaseDragSource()
    {
      foreach (var p in Position.OnBoard)
        GetCell(p).IsPossibleMoveTarget = false;

      foreach (var p in Position.OnBoard)
        GetCell(p).IsPromotionAllowed = false;

      foreach (var p in Position.OnBoard)
        GetCell(p).IsPromotionRecommended = false;

      MoveSource = null;
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
      DependencyProperty.RegisterAttached("IsFlipped", typeof(bool),
        typeof(ShogiBoard), new FrameworkPropertyMetadata(false,
          FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public bool IsFlipped
    {
      get { return (bool)GetValue(IsFlippedProperty); }
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
      DependencyProperty.Register("Board", typeof(Board),
       typeof(ShogiBoard), new UIPropertyMetadata(null, BoardPropertyChanged));

    private static void BoardPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
    {
      ((ShogiBoard)o).BoardPropertyChanged((Board)args.OldValue, (Board)args.NewValue);
    }

    private void BoardPropertyChanged(Board oldValue, Board newValue)
    {
      if (oldValue != null)
      {
        oldValue.Moving -= BoardOnMoving;
        oldValue.HistoryNavigating -= OnHistoryNavigating;
      }

      if (newValue != null)
      {
        newValue.Moving += BoardOnMoving;
        newValue.HistoryNavigating += OnHistoryNavigating;
      }
    }

    private void OnHistoryNavigating(object sender, HistoryNavigateEventArgs args)
    {
      var history = Board.History;
      if (args.Step == -1)
      {
        AnimateInvertedMove(history[history.CurrentMoveIndex - args.Step]);
      }
      else if (args.Step == 1)
      {
        AnimateMove(history.CurrentMove);
      }
    }

    public Board Board
    {
      get { return (Board)GetValue(BoardProperty); }
      set { SetValue(BoardProperty, value); }
    }

    #endregion

    #region ' AreMoveRulesEnforced Property '

    public static readonly DependencyProperty AreMoveRulesEnforcedProperty =
      DependencyProperty.Register("AreMoveRulesEnforced", typeof(bool),
        typeof(ShogiBoard), new UIPropertyMetadata(true, OnMoveRulesEnforcedChanged));


    private static void OnMoveRulesEnforcedChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
    {
      ((ShogiBoard)o).OnMoveRulesEnforcedChanged((bool)args.NewValue);
    }
    private void OnMoveRulesEnforcedChanged(bool value)
    {
      Board.IsMovesOrderMaintained = value;
    }

    public bool AreMoveRulesEnforced
    {
      get { return (bool)GetValue(AreMoveRulesEnforcedProperty); }
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

    #region ' Helpers '

    public ShogiHand this [PieceColor index]
    {
      get { return index == PieceColor.White ? WhiteHand : BlackHand; }
    }
    public ShogiCell GetCell(Position position)
    {
      return Core.GetCell(position);
    }

    #endregion

    #region ' Fields '

    private readonly Dnd _dnd;
    private readonly Flag _dragMove = new Flag();

    #endregion

    #region ' Internal Interface '

    /// <summary>This method is called by ShogiBoardCore itself</summary>
    internal void SetupShogiBoardCore(ShogiBoardCore core)
    {
      Core = core;
    }
    /// <summary>This method is called by ShogiHand itself</summary>
    internal void SetupShogiHand(ShogiHand hand)
    {
      _hands.Add(hand);
      Debug.Assert(_hands.Count < 3);
    }

    #endregion

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return new ShogiBoardAutomationPeer(this);
    }
  }
}