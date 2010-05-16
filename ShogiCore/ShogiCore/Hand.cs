using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Yasc.ShogiCore.Snapshots;
using Yasc.Utils;

namespace Yasc.ShogiCore
{
  /// <summary>Represents collection of pieces player has "in hand"</summary>
  public class Hand : ReadOnlyObservableCollection<Piece>
  {
    public void RemoveAt(int index)
    {
      Items.RemoveAt(index);
    }
    public bool Remove(Piece item)
    {
      return Items.Remove(item);
    }
    public void Add(Piece item)
    {
      Items.Add(item);
    }
    public void Clear()
    {
      Items.Clear();
    }
    public void Move(int oldIndex, int newIndex)
    {
      Items.Move(oldIndex, newIndex);
    }
    
    private readonly Board _board;
    private readonly Player _owner;

    private new ObservableCollection<Piece> Items
    {
      get { return (ObservableCollection<Piece>) base.Items; }
    }
    /// <summary>ctor</summary>
    public Hand(Board board, Player owner)
      : base(new ThreadSafeObservableCollection<Piece>())
    {
      _board = board;
      _owner = owner;
      Items.CollectionChanged += OnHandCollectionChanged;
    }

    private void OnHandCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
      switch (args.Action)
      {
        case NotifyCollectionChangedAction.Add:
          OnAddPieceToHand(args);
          break;
        case NotifyCollectionChangedAction.Remove:
          OnRemovePieceFromHand(args);
          break;
        case NotifyCollectionChangedAction.Move:
          // Piece doesn't come and doesn't leave.
          break;
        case NotifyCollectionChangedAction.Replace:
          OnRemovePieceFromHand(args);
          OnAddPieceToHand(args);
          break;
        default:
          throw new NotSupportedException(
            "Player.Hand collection doesn't support Reset change. " +
            "If you want to clear try Player.ResetAllPiecesFromHand");
      }
    }

    private void OnAddPieceToHand(NotifyCollectionChangedEventArgs args)
    {
      foreach (Piece p in args.NewItems)
      {
        if (p.Owner != null)
        {
          throw new InvalidOperationException(
            "Piece can't be in two places at the same time. " +
            "First return it to the PieceSet, then try to add it to the hand");
        }
        _board.PieceSet.Pop(p);
        p.Owner = _owner;
        p.IsPromoted = false;
      }
    }

    private void OnRemovePieceFromHand(NotifyCollectionChangedEventArgs args)
    {
      foreach (Piece p in args.OldItems)
        _board.PieceSet.Push(p);
    }

    /// <summary>Gets the piece from player hand by type</summary>
    public Piece GetByType(PieceType pieceType)
    {
      return Items.FirstOrDefault(piece => piece.PieceType == pieceType);
    }

    /// <summary>Adds the piece to player hand</summary>
    public Piece AddToHand(PieceType type)
    {
      Piece piece = _board.PieceSet[type];
      if (piece == null)
      {
        throw new NotEnoughPiecesInSetException(
          "Cannot add piece because there's no more pieces of type " +
          type + " in the set. Consider using Infinite PieceSet");
      }
      Items.Add(piece);
      return piece;
    }

    /// <summary>Load pieces to the hand from snapshot</summary>
    public void LoadSnapshot(IEnumerable<PieceSnapshot> handSnapshot)
    {
      if (handSnapshot == null) throw new ArgumentNullException("handSnapshot");
      Items.Update(handSnapshot,
                   p => new PieceSnapshot(p),
                   ps => ps,
                   ps => _board.PieceSet[ps.PieceType]);
    }

    /// <summary>Returns all pieces from hand to the set</summary>
    public void ResetAllPiecesFromHand()
    {
#warning some synchronizer doesn't support clear.
      while (Items.Count > 0)
        Items.RemoveAt(Items.Count - 1);
    }

  }
}