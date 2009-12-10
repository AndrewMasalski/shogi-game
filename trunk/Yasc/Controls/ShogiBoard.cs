using System;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using MvvmFoundation.Wpf;
using Yasc.Controls.Automation;
using Yasc.GenericDragDrop;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Moves;
using Yasc.Utils;

namespace Yasc.Controls
{
  [TemplatePart(Name = "PART_AdornerLayer", Type = typeof(Canvas))]
  [TemplatePart(Name = "PART_Shield", Type = typeof(ContentControl))]
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
    public ShogiHand WhiteHand { get; private set; }
    public ShogiHand BlackHand { get; private set; }

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
        AnimateMove(GetNest(d.PieceType, d.Who.Color), GetCell(d.To));
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
        AnimateMove(GetCell(d.To), GetNest(d.PieceType, d.Who.Color));
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
      var positions = from m in moves select m.To;

      foreach (var p in positions.Distinct())
        GetCell(p).IsPossibleMoveTarget = true;

      var promoting = from m in moves where m.IsPromoting select m.To;

      foreach (var p in promoting)
        GetCell(p).IsPromotionAllowed = true;

      if (args.Piece.Type != "銀")
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
      if (promotionRequest) piece.IsPromoted = true;
      piece.Owner.Hand.Remove(piece);
      var m = cell.Piece;
      cell.ResetPiece();
      cell.SetPiece(piece, piece.Owner);
      if (m != null) piece.Owner.Hand.Add(m);
    }
    private static void ArbitraryMove(Cell from, Cell to, bool promotionRequest)
    {
      var piece = from.Piece;
      if (promotionRequest) piece.IsPromoted = true;
      from.ResetPiece();
      var m = to.Piece;
      to.ResetPiece();
      to.SetPiece(piece, piece.Owner);
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

        IsCurrentMoveLast = newValue.History.IsCurrentMoveLast;
        _movesHistoryObserver = new PropertyObserver<MovesHistory>(newValue.History).
          RegisterHandler(h => h.IsCurrentMoveLast, h => IsCurrentMoveLast = h.IsCurrentMoveLast);
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

    #region ' IsCurrentMoveLast Property '

    public static readonly DependencyProperty IsCurrentMoveLastProperty =
      DependencyProperty.Register("IsCurrentMoveLast", typeof(bool),
        typeof(ShogiBoard), new UIPropertyMetadata(false, OnIsCurrentMoveLastPropertyChanged));

    private static void OnIsCurrentMoveLastPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ShogiBoard)d).OnIsCurrentMoveLastPropertyChanged((bool)e.NewValue);

    }
    private void OnIsCurrentMoveLastPropertyChanged(bool value)
    {
      if (!value)
      {
        Shield shieldControl;
        if (_shieldControl == null)
        {
          _shieldControl = new WeakReference<Shield>(shieldControl = CreateShield());
        }
        else
        {
          shieldControl = _shieldControl.Target;
        }

        if (shieldControl == null)
        {
          _shieldControl.Target = shieldControl = CreateShield();
        }

        _shieldContainer.Content = shieldControl;
      }
      else _shieldContainer.Content = null;
    }

    private Shield CreateShield()
    {
      var shield = new Shield();
      shield.MouseUp += (s, e) => Board.History.GoToTheLast();
      return shield;
    }

    public bool IsCurrentMoveLast
    {
      get { return (bool)GetValue(IsCurrentMoveLastProperty); }
      set { SetValue(IsCurrentMoveLastProperty, value); }
    }

    #endregion

    #region ' Parts '

    public override void OnApplyTemplate()
    {
      _adornerLayer = GetTemplateChild("PART_AdornerLayer") as Canvas;
      _shieldContainer = GetTemplateChild("PART_Shield") as ContentControl;
      base.OnApplyTemplate();
    }

    private Canvas _adornerLayer;
    private ContentControl _shieldContainer;

    #endregion

    #region ' Helpers '

    public HandNest GetNest(PieceType type, PieceColor color)
    {
      var hand = this.FindChild<ShogiHand>(h => h.Color == color);
      return hand.FindLastChild<HandNest>(n => n.PieceType == type);
    }
    public ShogiCell GetCell(Position position)
    {
      return Core.GetCell(position);
    }

    #endregion

    #region ' Fields '

    private WeakReference<Shield> _shieldControl;
    private readonly Dnd _dnd;
    private readonly Flag _dragMove = new Flag();
    /// <summary>Holds the reference to prevent GC from collecting</summary>
    // ReSharper disable UnaccessedField.Local
    private PropertyObserver<MovesHistory> _movesHistoryObserver;
    // ReSharper restore UnaccessedField.Local

    #endregion

    #region ' Internal Interface '

    /// <summary>This method is called by ShogiBoardCore itself</summary>
    internal void SetupShogiBoardCore(ShogiBoardCore core)
    {
      Core = core;
    }
    /// <summary>This method is called by ShogiHand itself</summary>
    internal void SetupShohiHand(ShogiHand hand)
    {
      if (hand.Color == PieceColor.White)
      {
        WhiteHand = hand;
      }
      else
      {
        BlackHand = hand;
      }
    }

    #endregion

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return new ShogiBoardAutomationPeer(this);
    }
  }
}