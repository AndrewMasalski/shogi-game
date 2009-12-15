using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Yasc.Controls;

namespace Yasc.GenericDragDrop
{
  internal class Dnd
  {
    public Dnd(ShogiBoard board)
    {
      _board = board;
      _board.PreviewMouseLeftButtonDown += MouseLeftDown;
    }

    #region ' Implementation '

    private DndAdorner Adorner
    {
      get
      {
        if (_adorner == null)
        {
          _adorner = new DndAdorner(_piece) { IsHitTestVisible = false };
        }
        return _adorner;
      }
    }
    private AdornerLayer AdornerLayer
    {
      get
      {
        if (_adornerLayer == null)
        {
          _adornerLayer = AdornerLayer.GetAdornerLayer(_piece);
        }
        return _adornerLayer;
      }
    }

    private void MouseLeftDown(object sender, MouseButtonEventArgs e)
    {
      _piece = (e.OriginalSource as DependencyObject).FindAncestor<ShogiPiece>();

      if (_piece == null) return;

      _wasOriginallyPromoted = _piece.IsPromoted;
      _initialMousePosition = e.GetPosition(_board);
      _board.PreviewMouseMove += MouseMove;
      _board.PreviewMouseLeftButtonUp += MouseLeftUp;
      _board.PreviewMouseRightButtonDown += MouseRightDown;
      _board.MouseLeave += OnMouseLeaveBoard;

      var cell = _piece.FindAncestor<ShogiCell>();
      if (cell != null)
      {
        _dragFrom = OnDragFromBoard(new DragFromBoardEventArgs(_piece, cell));
      }
      else
      {
        _dragFrom = OnDragFromHand(new DragFromHandEventArgs(_board, _piece));
      }
    }

    private void MouseRightDown(object sender, MouseButtonEventArgs e)
    {
      if (_piece == null || _wasOriginallyPromoted) return;

      var cell = (e.OriginalSource as DependencyObject).FindAncestor<ShogiCell>();
      if (cell == null || !cell.IsPromotionAllowed)
      {
        _promotionUserChoice = false;
        _piece.IsPromoted = false;
      }
      else
      {
        _promotionUserChoice = !_piece.IsPromoted;
        _piece.IsPromoted = !_piece.IsPromoted;
      }
    }
    private void MouseMove(object sender, MouseEventArgs e)
    {
      if (!_adornerIsShown)
      {
        AdornerLayer.Add(Adorner);
        _adornerIsShown = true;
      }

      Adorner.Offset = e.GetPosition(_board) - _initialMousePosition;

      if (_wasOriginallyPromoted) return;
      var cell = (e.OriginalSource as DependencyObject).FindAncestor<ShogiCell>();
      if (cell == null) return;
      if (!cell.IsPromotionAllowed)
      {
        _piece.IsPromoted = false;
      }
      else if (_promotionUserChoice != null)
      {
        _piece.IsPromoted = (bool)_promotionUserChoice;
      }
      else if (cell.IsPromotionRecommended)
      {
        _piece.IsPromoted = true;
      }
    }
    private void MouseLeftUp(object sender, MouseEventArgs e)
    {
      try
      {
        var result = VisualTreeHelper.HitTest(_board, e.GetPosition(_board));
        if (result == null) return;
        var cell = result.VisualHit.FindAncestor<ShogiCell>();
        var hand = result.VisualHit.FindAncestor<ShogiHand>();
        Debug.Assert(cell == null || hand == null);
        if (cell != null) OnDropToBoard(new DropToBoardEventArgs(_dragFrom, cell, _piece.IsPromoted != _wasOriginallyPromoted));
        if (hand != null) OnDropToHand(new DropToHandEventArgs(_dragFrom, hand));
      }
      finally
      {
        Release();
      }
    }
    private void Release()
    {
      if (!_successfulDrop) OnDragCancelled(_dragFrom);

      _dragFrom = null;
      _piece = null;
      _successfulDrop = false;
      _promotionUserChoice = null;

      _board.PreviewMouseMove -= MouseMove;
      _board.PreviewMouseLeftButtonUp -= MouseLeftUp;
      _board.PreviewMouseRightButtonDown -= MouseRightDown;
      _board.MouseLeave -= OnMouseLeaveBoard;

      if (!_adornerIsShown) return;
      _adornerLayer.Remove(_adorner);
      _adornerIsShown = false;
      _adorner = null;
    }

    private void OnMouseLeaveBoard(object sender, MouseEventArgs e)
    {
      Release();
    }
    #endregion

    #region ' Fields '

    private ShogiBoard _board;
    private DndAdorner _adorner;
    private AdornerLayer _adornerLayer;

    private Point _initialMousePosition;
    private bool _adornerIsShown;
    private ShogiPiece _piece;

    private DragFromEventArgs _dragFrom;
    private bool _successfulDrop;
    private bool _wasOriginallyPromoted;
    private bool? _promotionUserChoice;

    #endregion

    #region ' Events '

    public event EventHandler<DragFromBoardEventArgs> DragFromBoard;
    public event EventHandler<DragFromHandEventArgs> DragFromHand;
    public event EventHandler<DropToBoardEventArgs> DropToBoard;
    public event EventHandler<DropToHandEventArgs> DropToHand;
    public event EventHandler<DragFromEventArgs> DragCancelled;

    private DragFromBoardEventArgs OnDragFromBoard(DragFromBoardEventArgs e)
    {
      var drag = DragFromBoard;
      if (drag != null) drag(this, e);
      return e;
    }
    private DragFromHandEventArgs OnDragFromHand(DragFromHandEventArgs e)
    {
      var drag = DragFromHand;
      if (drag != null) drag(this, e);
      return e;
    }
    private void OnDropToBoard(DropToBoardEventArgs e)
    {
      _piece.IsPromoted = _wasOriginallyPromoted;
      _successfulDrop = true;
      var drag = DropToBoard;
      if (drag != null) drag(this, e);
    }
    private void OnDropToHand(DropToHandEventArgs e)
    {
      _piece.IsPromoted = _wasOriginallyPromoted;
      _successfulDrop = true;
      var drag = DropToHand;
      if (drag != null) drag(this, e);
    }
    private void OnDragCancelled(DragFromEventArgs e)
    {
      if (_piece != null)
      {
        _piece.IsPromoted = _wasOriginallyPromoted;
      }
      var cancelled = DragCancelled;
      if (cancelled != null)
      {
        cancelled(this, e);
      }
    }

    #endregion
  }
}