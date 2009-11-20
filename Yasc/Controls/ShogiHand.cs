using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Utils;

namespace Yasc.Controls
{
  public class ShogiHand : Control
  {
    static ShogiHand()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ShogiHand),
        new FrameworkPropertyMetadata(typeof(ShogiHand)));
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
      ((ShogiHand)d).OnHandChanged((ObservableCollection<Piece>)e.OldValue, (ObservableCollection<Piece>)e.NewValue);
    }

    private void OnHandChanged(INotifyCollectionChanged oldCollection, ObservableCollection<Piece> newCollection)
    {
      if (oldCollection != null)
      {
        oldCollection.CollectionChanged -= OnHandCollectionChanged;
      }
      if (newCollection != null)
      {
        newCollection.CollectionChanged += OnHandCollectionChanged;
      }
      Items = GetItems(newCollection);
    }

    private ObservableCollection<HandNest> GetItems(IEnumerable<Piece> collection)
    {
      if (collection == null) return null;
      return new ObservableCollection<HandNest>(
        from p in collection
        select new HandNest { PieceColor = Color, PieceType = p.Type, PiecesCount = 1 });
    }

    private void OnHandCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
      switch (args.Action)
      {
        case NotifyCollectionChangedAction.Add:
          int i = 0;
          foreach (Piece piece in args.NewItems)
            Items.Insert(args.NewStartingIndex + i++, new HandNest
            {
              PieceColor = Color,
              PieceType = piece.Type,
              PiecesCount = 1
            });
          break;
        case NotifyCollectionChangedAction.Remove:
          for (int j = 0; j < args.OldItems.Count; j++)
            Items.RemoveAt(args.OldStartingIndex);
          break;
        default:
          throw new NotSupportedException();
      }
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
  }
}