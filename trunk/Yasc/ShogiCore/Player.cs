using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Yasc.ShogiCore.SnapShots;
using Yasc.Utils;

namespace Yasc.ShogiCore
{
  public class Player
  {
    public Board Board { get; private set; }
    public string Name { get; set; }
    public ObservableCollection<Piece> Hand { get; set; }
    
    internal Player(Board board)
    {
      Board = board;
      Hand = new ThreadSafeObservableCollection<Piece>();
      Hand.CollectionChanged += OnHandCollectionChanged;
    }

    public Player Oppenent
    {
      get { return Board.Black == this ? Board.White : Board.Black; }
    }
    public PieceColor Color
    {
      get { return Board.White == this ? PieceColor.White : PieceColor.Black; }
    }
    public Piece GetPieceFromHandByType(PieceType pieceType)
    {
      foreach (var piece in Hand)
        if (piece.Type == pieceType)
          return piece;
      return null;
    }
    public Piece AddToHand(PieceType type)
    {
      Piece piece = Board.PieceSet[type];
      Hand.Add(piece);
      return piece;
    }

    public void LoadSnapshot(IEnumerable<PieceSnapshot> collection)
    {
#warning Implement cool update. See ListUtils.Update
      ResetAllPiecesFromHand();
      foreach (var snapshot in collection)
        AddToHand(snapshot.Type);
    }

    public void ResetAllPiecesFromHand()
    {
#warning some synchronizer doesn't support clear.

      while (Hand.Count > 0)
      {
        var piece = Hand[Hand.Count - 1];
        Hand.RemoveAt(Hand.Count - 1);
        Board.PieceSet.Return(piece);
      }
    }

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
          foreach (Piece p in args.NewItems)
          {
            if (p.Owner != null) 
            {
              throw new InvalidOperationException(
                "Piece can't be in two places at the same time. " +
                "First return it to the PieceSet, then try to add to the hand");
            }
            Board.PieceSet.Take(p);
            p.Owner = this;
            p.IsPromoted = false;
          }
          break;
        case NotifyCollectionChangedAction.Remove:
          foreach (Piece p in args.OldItems)
            Board.PieceSet.Return(p);
          break;
      }
    }

    #endregion
  }
}