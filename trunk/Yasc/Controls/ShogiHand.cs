﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using Yasc.Controls.Automation;
using Yasc.ShogiCore;
using Yasc.GenericDragDrop;

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

    #region ' GroupingMode Property '

    public static readonly DependencyProperty IsGroupingProperty =
      DependencyProperty.Register(
      "GroupingMode", typeof (HandGroupingMode), typeof(ShogiHand), 
      new UIPropertyMetadata(HandGroupingMode.Plain, OnIsGroupingChanged));

    private static void OnIsGroupingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ShogiHand)d).OnIsGroupingChanged((HandGroupingMode)e.NewValue);
    }

    private void OnIsGroupingChanged(HandGroupingMode value)
    {
      _synchStrategy.Close();
      _synchStrategy = GetStrategy(value);
    }

    private SynchStrategy GetStrategy(HandGroupingMode value)
    {
      switch (value)
      {
        case HandGroupingMode.OrderedGroups:
          return new OrderedGroupsSynch(this, Hand);
        case HandGroupingMode.Groups:
          return new GroupsSynch(this, Hand);
        default:
          return new PlainSynch(this, Hand);
      }
    }

    public HandGroupingMode GroupingMode
    {
      get { return (HandGroupingMode)GetValue(IsGroupingProperty); }
      set { SetValue(IsGroupingProperty, value); }
    }

    #endregion

    #region ' Synch Strategies '

    private abstract class SynchStrategy : IWeakEventListener
    {
      protected ShogiHand _owner;
      private INotifyCollectionChanged _src;

      public void OnHandChanged(ObservableCollection<Piece> collection)
      {
        if (_src != null)
        {
          CollectionChangedEventManager.RemoveListener(_src, this);
        }
        _src = collection;
        if (_src != null)
        {
          CollectionChangedEventManager.AddListener(_src, this);
        }
        _owner.Items = GetItems(collection);
      }

      protected abstract ObservableCollection<HandNest> GetItems(IEnumerable<Piece> collection);
      protected abstract void OnHandCollectionChanged(NotifyCollectionChangedEventArgs args);

      public void Close()
      {
        if (_src != null) CollectionChangedEventManager.RemoveListener(_src, this);
      }

      public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
      {
        OnHandCollectionChanged((NotifyCollectionChangedEventArgs) e);
        return true;
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
          select new HandNest
                   {
                     PieceColor = _owner.Color,
                     PieceType = p.PieceType, 
                     PiecesCount = 1
                   });
      }

      protected override void OnHandCollectionChanged(NotifyCollectionChangedEventArgs args)
      {
        switch (args.Action)
        {
          case NotifyCollectionChangedAction.Add:
            int i = 0;
            foreach (Piece piece in args.NewItems)
              _owner.Items.Insert(args.NewStartingIndex + i++, new HandNest
              {
                PieceColor = _owner.Color,
                PieceType = piece.PieceType,
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

    private sealed class GroupsSynch : SynchStrategy
    {
      public GroupsSynch(ShogiHand owner, ObservableCollection<Piece> collection)
      {
        _owner = owner;
        OnHandChanged(collection);
      }

      protected override ObservableCollection<HandNest> GetItems(IEnumerable<Piece> collection)
      {
        if (collection == null) return null;
        var r = from p in collection
                group p by p.PieceType into g
                select new HandNest
                  {
                    PieceColor = _owner.Color,
                    PieceType = g.Key,
                    PiecesCount = g.Count()
                  };
        return new ObservableCollection<HandNest>(r);
      }

      protected override void OnHandCollectionChanged(NotifyCollectionChangedEventArgs args)
      {
        switch (args.Action)
        {
          case NotifyCollectionChangedAction.Add:
            foreach (Piece piece in args.NewItems)
              FindOrCreateNest(piece.PieceType).PiecesCount++;
            break;
          case NotifyCollectionChangedAction.Remove:
            foreach (Piece piece in args.OldItems)
            {
              var nest = FindOrCreateNest(piece.PieceType);
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

    private sealed class OrderedGroupsSynch : SynchStrategy
    {
      public OrderedGroupsSynch(ShogiHand owner, ObservableCollection<Piece> collection)
      {
        _owner = owner;
        OnHandChanged(collection);
      }

      protected override ObservableCollection<HandNest> GetItems(IEnumerable<Piece> collection)
      {
        if (collection == null) return null;
        return new ObservableCollection<HandNest>(
          from id in PieceType.GetIds()
          let pieceType = PieceType.GetPieceType(id)
          select new HandNest
                   {
                     PieceColor = _owner.Color,
                     PieceType = pieceType,
                     PiecesCount = (from p in collection
                                    where p.PieceType == pieceType
                                    select p).Count()
                   });
      }

      protected override void OnHandCollectionChanged(NotifyCollectionChangedEventArgs args)
      {
        switch (args.Action)
        {
          case NotifyCollectionChangedAction.Add:
            foreach (Piece piece in args.NewItems)
              _owner.Items[piece.PieceType.Id].PiecesCount++;
            break;
          case NotifyCollectionChangedAction.Remove:
            foreach (Piece piece in args.OldItems)
              _owner.Items[piece.PieceType.Id].PiecesCount--;
            break;
          default:
            throw new NotSupportedException();
        }
      }
    }

    #endregion

    public override void OnApplyTemplate()
    {
      var board = TemplatedParent.FindAncestor<ShogiBoard>();
      if (board != null) board.SetupShohiHand(this);
      base.OnApplyTemplate();
    }
    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return new ShogiHandAutomationPeer(this);
    }
  }
}