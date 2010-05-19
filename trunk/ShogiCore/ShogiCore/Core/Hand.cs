using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Yasc.ShogiCore.Primitives;
using Yasc.ShogiCore.Snapshots;
using Yasc.Utils;

namespace Yasc.ShogiCore.Core
{
  /// <summary>Represents collection of pieces player has "in hand"</summary>
  public class Hand : ReadOnlyObservableCollection<Piece>
  {
    #region ' Fields '

    private readonly IPieceSet _pieceSet;
    private readonly Player _owner;

    #endregion

    #region ' Public Interface '

    /// <summary>ctor</summary>
    public Hand(IPieceSet pieceSet, Player owner)
      : base(new PieceCollection())
    {
      _pieceSet = pieceSet;
      _owner = owner;
      Items.FirstCalledCollectionChanged += OnCollectionChanged;
    }

    /// <summary>Adds the piece from board's piece set to the hand</summary>
    public Piece Add(PieceType type)
    {
      var piece = _pieceSet[type];
      if (piece == null)
      {
        throw new NotEnoughPiecesInSetException(
          "Cannot add piece because there's no more pieces of type " +
          type + " in the set. Consider using Infinite PieceSet");
      }
      Items.Add(piece);
      return piece;
    }
    /// <summary>Adds the piece to the hand</summary>
    public void Add(Piece item)
    {
      Items.Add(item);
    }

    /// <summary>Gets the piece from player hand by type -or- null</summary>
    public Piece GetByType(PieceType pieceType)
    {
      return Items.FirstOrDefault(piece => piece.PieceType == pieceType);
    }
    /// <summary>Load pieces to the hand from snapshot</summary>
    public void LoadSnapshot(IEnumerable<PieceSnapshot> handSnapshot)
    {
      if (handSnapshot == null) throw new ArgumentNullException("handSnapshot");
      Items.Update(handSnapshot, p => p.Snapsot(), ps => ps, ps => _pieceSet[ps.PieceType]);
    }

    /// <summary>Removes the piece from hand</summary>
    public bool Remove(Piece item)
    {
      return Items.Remove(item);
    }
    /// <summary>Removes the piece from hand by type</summary>
    public bool Remove(PieceType pieceType)
    {
      var item = GetByType(pieceType);
      return item != null && Items.Remove(item);
    }

    /// <summary>Returns all pieces from hand to the set</summary>
    public void Clear()
    {
      // TODO: Replace with clear. Now some synchronizer doesn't support it
      while (Items.Count > 0)
        Items.RemoveAt(Items.Count - 1);
    }

    #endregion

    #region ' Nested: PieceCollection '

    class PieceCollection : ThreadSafeObservableCollection<Piece>
    {
      public NotifyCollectionChangedEventHandler FirstCalledCollectionChanged { get; set; }

      protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
      {
        using (BlockReentrancy())
        {
          var handler = FirstCalledCollectionChanged;
          if (handler != null) handler(this, e);

          base.OnCollectionChanged(e);
        }
      }
    }

    #endregion

    #region ' Implementation '

    private new PieceCollection Items
    {
      get { return (PieceCollection)base.Items; }
    }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
      switch (args.Action)
      {
        case NotifyCollectionChangedAction.Add:
          OnPieceAdded(args);
          break;
        case NotifyCollectionChangedAction.Remove:
          OnPieceRemoved(args);
          break;
        case NotifyCollectionChangedAction.Move:
          // Piece doesn't come and doesn't leave.
          break;
        case NotifyCollectionChangedAction.Replace:
          OnPieceRemoved(args);
          OnPieceAdded(args);
          break;
        default:
          throw new NotSupportedException(
            "Player.Hand collection doesn't support Reset change. " +
            "If you want to clear try Player.ResetAllPiecesFromHand");
      }
    }
    private void OnPieceAdded(NotifyCollectionChangedEventArgs args)
    {
      foreach (Piece p in args.NewItems)
      {
        if (p.Owner != null)
        {
          throw new InvalidOperationException(
            "Piece can't be in two places at the same time. " +
            "First return it to the PieceSet, then try to add it to the hand");
        }
        _pieceSet.Pop(p);
        p.Owner = _owner;
        p.IsPromoted = false;
      }
    }
    private void OnPieceRemoved(NotifyCollectionChangedEventArgs args)
    {
      foreach (Piece p in args.OldItems)
        _pieceSet.Push(p);
    }

    #endregion
  }
}