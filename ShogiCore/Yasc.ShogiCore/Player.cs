using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Yasc.ShogiCore.Snapshots;
using Yasc.Utils;

namespace Yasc.ShogiCore
{
  /// <summary>Game participiant</summary>
  public class Player
  {
    /// <summary>Board game is going on</summary>
    public Board Board { get; private set; }
    /// <summary>The player name</summary>
    public string Name { get; set; }
    /// <summary>The pieces player has in hand</summary>
    public ObservableCollection<Piece> Hand { get; set; }

    internal Player(Board board)
    {
      Board = board;
      Hand = new ThreadSafeObservableCollection<Piece>();
      Hand.CollectionChanged += OnHandCollectionChanged;
    }

    /// <summary>The player opponent</summary>
    public Player Opponent
    {
      get { return Board.Black == this ? Board.White : Board.Black; }
    }
    /// <summary>The player color</summary>
    public PieceColor Color
    {
      get { return Board.White == this ? PieceColor.White : PieceColor.Black; }
    }
    /// <summary>Gets the piece from player hand by type</summary>
    public Piece GetPieceFromHandByType(PieceType pieceType)
    {
      foreach (var piece in Hand)
        if (piece.PieceType == pieceType)
          return piece;
      return null;
    }
    /// <summary>Adds the piece to player hand</summary>
    public Piece AddToHand(PieceType type)
    {
      Piece piece = Board.PieceSet[type];
      if (piece == null)
      {
        throw new NotEnoughPiecesInSetException(
          "Cannot add piece because there's no more pieces of type " +
          type + " in the set. Consider using Infinite PieceSet");
      }
      Hand.Add(piece);
      return piece;
    }

    /// <summary>Load pieces to the hand from snapshot</summary>
    public void LoadHandSnapshot(IEnumerable<PieceSnapshot> handSnapshot)
    {
      if (handSnapshot == null) throw new ArgumentNullException("handSnapshot");
      Hand.Update(handSnapshot, 
        p => new PieceSnapshot(p), 
        ps => ps, 
        ps => Board.PieceSet[ps.PieceType]);
    }

    /// <summary>Returns all pieces from hand to the set</summary>
    public void ResetAllPiecesFromHand()
    {
#warning some synchronizer doesn't support clear.
      while (Hand.Count > 0)
        Hand.RemoveAt(Hand.Count - 1);
    }

    /// <summary>Get human readable representation of the player</summary>
    public override string ToString()
    {
      return Name ?? Color.ToString();
    }

    #region ' Ipmlementation '

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
        Board.PieceSet.Pop(p);
        p.Owner = this;
        p.IsPromoted = false;
      }
    }

    private void OnRemovePieceFromHand(NotifyCollectionChangedEventArgs args)
    {
      foreach (Piece p in args.OldItems)
        Board.PieceSet.Push(p);
    }

    #endregion
  }
}