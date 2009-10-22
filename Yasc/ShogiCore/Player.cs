using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Yasc.ShogiCore.Utils;
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

    private void OnHandCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
      if (args.Action == NotifyCollectionChangedAction.Add)
        foreach (Piece p in args.NewItems)
        {
          p.Owner = this;
          p.IsPromoted = false;
        }
    }
    public Player Oppenent
    {
      get { return Board.Black == this ? Board.White : Board.Black; }
    }
    public PieceColor Color
    {
      get { return Board.White == this ? PieceColor.White : PieceColor.Black; }
    }

    public override string ToString()
    {
      return Name ?? Color.ToString();
    }

    public Piece GetPieceFromHandByType(PieceType pieceType)
    {
      foreach (var piece in Hand)
        if (piece.Type == pieceType)
          return piece;
      return null;
    }
  }
}