using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Yasc.ShogiCore;

namespace Yasc.Controls
{
  public class ShogiHand : Control
  {
    private SynchStrategy _synchStrategy;

    static ShogiHand()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ShogiHand),
        new FrameworkPropertyMetadata(typeof(ShogiHand)));
    }
    public ShogiHand()
    {
      _synchStrategy = new PlainSynch(this, Hand);
    }

    #region ' Board Property '

    public static readonly DependencyProperty BoardProperty =
      DependencyProperty.Register("Board", typeof(Board),
        typeof(ShogiHand), new UIPropertyMetadata(null, OnBoardChanged));

    private static void OnBoardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ShogiHand)d).OnBoardChanged((Board)e.NewValue);
    }

    private void OnBoardChanged(Board board)
    {
      Hand = board == null ? null : board[Color].Hand;
    }

    public Board Board
    {
      get { return (Board)GetValue(BoardProperty); }
      set { SetValue(BoardProperty, value); }
    }


    #endregion

    #region ' Color Property '

    public static readonly DependencyProperty ColorProperty =
      DependencyProperty.Register("Color", typeof(PieceColor),
        typeof(ShogiHand), new UIPropertyMetadata(default(PieceColor), OnColorChanged));

    private static void OnColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
    {
      ((ShogiHand)o).OnColorChanged((PieceColor)args.NewValue);
    }

    private void OnColorChanged(PieceColor color)
    {
      Hand = Board == null ? null : Board[color].Hand;
    }

    public PieceColor Color
    {
      get { return (PieceColor)GetValue(ColorProperty); }
      set { SetValue(ColorProperty, value); }
    }

    #endregion

    #region ' Hand Property '

    public static readonly DependencyProperty HandProperty =
      DependencyProperty.Register("Hand", typeof(ObservableCollection<Piece>),
        typeof(ShogiHand), new UIPropertyMetadata(null, OnHandChanged));

    private static void OnHandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ShogiHand)d)._synchStrategy.OnHandChanged((ObservableCollection<Piece>)e.NewValue);
    }

    public ObservableCollection<Piece> Hand
    {
      get { return (ObservableCollection<Piece>)GetValue(HandProperty); }
      set { SetValue(HandProperty, value); }
    }

    #endregion

    #region ' Items Property '

    public static readonly DependencyProperty ItemsProperty =
      DependencyProperty.Register("Items", typeof(ObservableCollection<HandNest>),
        typeof(ShogiHand), new UIPropertyMetadata(default(ObservableCollection<HandNest>)));

    public ObservableCollection<HandNest> Items
    {
      get { return (ObservableCollection<HandNest>)GetValue(ItemsProperty); }
      set { SetValue(ItemsProperty, value); }
    }

    #endregion

    #region ' IsGrouping Property '

    public static readonly DependencyProperty IsGroupingProperty =
      DependencyProperty.Register("IsGrouping", typeof (bool),
                                  typeof(ShogiHand), new UIPropertyMetadata(false, OnIsGroupingChanged));

    private static void OnIsGroupingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ShogiHand) d).OnIsGroupingChanged((bool) e.NewValue);
    }

    private void OnIsGroupingChanged(bool value)
    {
      _synchStrategy.Dispose();
      _synchStrategy = value ? new GroupSynch(this, Hand) : 
                                                            (SynchStrategy) new PlainSynch(this, Hand);
    }

    public bool IsGrouping
    {
      get { return (bool) GetValue(IsGroupingProperty); }
      set { SetValue(IsGroupingProperty, value); }
    }

    #endregion

    #region ' Synch Strategies '

    private abstract class SynchStrategy : IDisposable
    {
      protected ShogiHand _owner;
      private INotifyCollectionChanged _src;

      public void OnHandChanged(ObservableCollection<Piece> collection)
      {
        if (_src != null)
        {
          _src.CollectionChanged -= OnHandCollectionChanged;
        }
        _src = collection;
        if (_src != null)
        {
          _src.CollectionChanged += OnHandCollectionChanged;
        }
        _owner.Items = GetItems(collection);
      }

      protected abstract ObservableCollection<HandNest> GetItems(IEnumerable<Piece> collection);
      protected abstract void OnHandCollectionChanged(object sender, NotifyCollectionChangedEventArgs args);

      public void Dispose()
      {
        if (_src != null) _src.CollectionChanged -= OnHandCollectionChanged;
      }
    }

    private sealed class PlainSynch : SynchStrategy
    {
      public PlainSynch(ShogiHand owner, ObservableCollection<Piece> collection)
      {
        _owner = owner;
        OnHandChanged(collection);
      }

      protected override ObservableCollection<HandNest> GetItems(IEnumerable<Piece> collection)
      {
        if (collection == null) return null;
        return new ObservableCollection<HandNest>(
          from p in collection
          select new HandNest { PieceColor = _owner.Color, PieceType = p.Type, PiecesCount = 1 });
      }

      protected override void OnHandCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
      {
        switch (args.Action)
        {
          case NotifyCollectionChangedAction.Add:
            int i = 0;
            foreach (Piece piece in args.NewItems)
              _owner.Items.Insert(args.NewStartingIndex + i++, new HandNest
              {
                PieceColor = _owner.Color,
                PieceType = piece.Type,
                PiecesCount = 1
              });
            break;
          case NotifyCollectionChangedAction.Remove:
            for (int j = 0; j < args.OldItems.Count; j++)
              _owner.Items.RemoveAt(args.OldStartingIndex);
            break;
          default:
            throw new NotSupportedException();
        }
      }
    }

    private sealed class GroupSynch : SynchStrategy
    {
      public GroupSynch(ShogiHand owner, ObservableCollection<Piece> collection)
      {
        _owner = owner;
        OnHandChanged(collection);
      }

      protected override ObservableCollection<HandNest> GetItems(IEnumerable<Piece> collection)
      {
        if (collection == null) return null;
        var r = from p in collection group p by p.Type into g select 
                  new HandNest {PieceColor = _owner.Color, PieceType = g.Key, PiecesCount = g.Count()};
        return new ObservableCollection<HandNest>(r);
      }

      protected override void OnHandCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
      {
        switch (args.Action)
        {
          case NotifyCollectionChangedAction.Add:
            foreach (Piece piece in args.NewItems)
              FindOrCreateNest(piece.Type).PiecesCount++;
            break;
          case NotifyCollectionChangedAction.Remove:
            foreach (Piece piece in args.OldItems)
            {
              var nest = FindOrCreateNest(piece.Type);
              if (nest.PiecesCount == 1)
              {
                _owner.Items.Remove(nest);
              }
              else if (nest.PiecesCount > 1)
              {
                nest.PiecesCount--;
              }
              else Debug.Fail("Why are we removing piece which was not in collection?");
            }
            break;
          default:
            throw new NotSupportedException();
        }
      }

      private HandNest FindOrCreateNest(PieceType type)
      {
        foreach (var nest in _owner.Items)
          if (nest.PieceType == type)
            return nest;

        var res = new HandNest
                    {
                      PieceColor = _owner.Color,
                      PieceType = type,
                      PiecesCount = 0
                    };
        _owner.Items.Add(res);
        return res;
      }
    }

    #endregion
  }
}