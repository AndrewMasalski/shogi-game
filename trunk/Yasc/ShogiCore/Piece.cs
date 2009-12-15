using MvvmFoundation.Wpf;

namespace Yasc.ShogiCore
{
  public class Piece : ObservableObject
  {
    public Player Owner { get; set; }
    public PieceType PieceType { get; private set; }
    public PieceColor Color
    {
      get { return Owner.Color; }
    }
    public bool IsPromoted
    {
      get { return PieceType.IsPromoted; }
      set
      {
        if (value == IsPromoted) return;
        PieceType = value ? PieceType.Promote() : PieceType.Unpromote();
        RaisePropertyChanged("IsPromoted");
        RaisePropertyChanged("PieceType");
      }
    }

    internal Piece(PieceType type)
    {
      PieceType = type;
    }

    public override string ToString()
    {
      return (Owner == null ? "Ownerless" : Owner.Color.ToString()) + 
        (IsPromoted ? " promoted " : " ") + PieceType;
    }
    public string ToLatinString()
    {
      return (Owner == null ? "Ownerless" : Owner.Color.ToString()) + 
        (IsPromoted ? " promoted " : " ") + PieceType.Latin;
    }
  }
}