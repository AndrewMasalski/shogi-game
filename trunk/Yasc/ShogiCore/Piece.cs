using MvvmFoundation.Wpf;

namespace Yasc.ShogiCore
{
  public class Piece : ObservableObject
  {
    public Player Owner { get; set; }
    public PieceType Type { get; private set; }
    public PieceColor Color
    {
      get { return Owner.Color; }
    }
    public bool IsPromoted
    {
      get { return Type.IsPromoted; }
      set
      {
        if (value == IsPromoted) return;
        Type = value ? Type.Promote() : Type.Unpromote();
        RaisePropertyChanged("IsPromoted");
        RaisePropertyChanged("Type");
      }
    }

    internal Piece(PieceType type)
    {
      Type = type;
    }

    public override string ToString()
    {
      return (Owner == null ? "Ownerless" : Owner.Color.ToString()) + 
        (IsPromoted ? " promoted " : " ") + Type;
    }
    public string ToLatinString()
    {
      return (Owner == null ? "Ownerless" : Owner.Color.ToString()) + 
        (IsPromoted ? " promoted " : " ") + Type.Latin;
    }
  }
}