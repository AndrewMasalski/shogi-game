using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Yasc.Controls;

namespace Yasc.GenericDragDrop
{
  public class Dnd : IDisposable
  {
    public Dnd(ShogiBoard board)
    {
      _board = board;
      _board.PreviewMouseLeftButtonDown += MouseDown;
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

    private void MouseDown(object sender, MouseButtonEventArgs e)
    {
      _piece = (e.OriginalSource as DependencyObject).FindAncestor<ShogiPiece>();
      if (_piece == null) return;

      _initialMousePosition = e.GetPosition(_board);
      _board.PreviewMouseMove += MouseMove;
      _board.PreviewMouseUp += MouseUp;
      _board.CaptureMouse();

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
    private void MouseMove(object sender, MouseEventArgs e)
    {
      if (!_adornerIsShown)
      {
        AdornerLayer.Add(Adorner);
        _adornerIsShown = true;
      }

      Adorner.Offset = e.GetPosition(_board) - _initialMousePosition;
    }
    private void MouseUp(object sender, MouseEventArgs e)
    {
      try
      {
        var result = VisualTreeHelper.HitTest(_board, e.GetPosition(_board));
        var cell = result.VisualHit.FindAncestor<ShogiCell>();
        var hand = result.VisualHit.FindAncestor<ShogiHand>();
        Debug.Assert(cell == null || hand == null);
        if (cell != null) OnDropToBoard(new DropToBoardEventArgs(_dragFrom, cell));
        if (hand != null) OnDropToHand(new DropToHandEventArgs(_dragFrom, hand));
        if (cell == null && hand == null) OnDragCancelled(_dragFrom);
        result.ToString();
      }
      finally
      {
        Release();
      }
    }
    private void Release()
    {
      _dragFrom = null;
      _piece = null;

      _board.ReleaseMouseCapture();
      _board.PreviewMouseMove -= MouseMove;
      _board.PreviewMouseUp -= MouseUp;

      if (!_adornerIsShown) return;
      _adornerLayer.Remove(_adorner);
      _adornerIsShown = false;
      _adorner = null;
    }

    #endregion

    public void Dispose()
    {
      _board.PreviewMouseLeftButtonDown -= MouseDown;
      _board = null;

      _adornerLayer = null;
    }

    #region ' Fields '

    private ShogiBoard _board;
    private DndAdorner _adorner;
    private AdornerLayer _adornerLayer;

    private Point _initialMousePosition;
    private bool _adornerIsShown;
    private ShogiPiece _piece;

    private DragFromEventArgs _dragFrom;

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
      var drag = DropToBoard;
      if (drag != null) drag(this, e);
    }
    private void OnDropToHand(DropToHandEventArgs e)
    {
      var drag = DropToHand;
      if (drag != null) drag(this, e);
    }
    private void OnDragCancelled(DragFromEventArgs e)
    {
      var cancelled = DragCancelled;
      if (cancelled != null) cancelled(this, e);
    }

    #endregion
  }
}