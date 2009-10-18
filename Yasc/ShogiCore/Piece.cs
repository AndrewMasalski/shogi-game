using Yasc.ShogiCore.Utils;

namespace Yasc.ShogiCore
{
  public class Piece : ViewModelBase
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
        Type = value ? Type.Unpromote() : Type.Promote();
        OnPropertyChanged("IsPromoted");
      }
    }

    public Piece(Player owner, PieceType type)
    {
      Owner = owner;
      Type = type;
    }
  }
}