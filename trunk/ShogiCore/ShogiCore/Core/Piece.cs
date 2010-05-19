using Yasc.ShogiCore.Primitives;
using Yasc.ShogiCore.Snapshots;
using Yasc.Utils.Mvvm;

namespace Yasc.ShogiCore.Core
{
  /// <summary>Represents observable piece with owner, type, color, etc.</summary>
  public class Piece : ObservableObject
  {
    /// <summary>The piece owner</summary>
    public Player Owner { get; set; }
    /// <summary>The piece type</summary>
    public PieceType PieceType { get; private set; }
    /// <summary>Current piece color</summary>
    /// <remarks>Makes no sense if there's no <see cref="Owner"/></remarks>
    public PieceColor Color
    {
      get { return Owner.Color; }
    }
    /// <summary>Indicates whether the piece is promoted</summary>
    /// <remarks>When you set it <see cref="PieceType"/> changes accordingly</remarks>
    public bool IsPromoted
    {
      get { return PieceType.IsPromoted; }
      set
      {
        if (value == IsPromoted) return;
        PieceType = value ? PieceType.Promote() : PieceType.Demote();
        RaisePropertyChanged("IsPromoted");
        RaisePropertyChanged("PieceType");
      }
    }

    internal Piece(PieceType type)
    {
      PieceType = type;
    }

    /// <summary>Takes a snapshot of the piece</summary>
    public PieceSnapshot Snapshot()
    {
      return new PieceSnapshot(PieceType, Color);
    }

    /// <summary>Gets user friendly piece name with japanese type for debug purposes</summary>
    /// <returns>e.g. Ownerless 飛</returns>
    public override string ToString()
    {
      return (Owner == null ? "Ownerless" : Owner.Color.ToString()) + 
        (IsPromoted ? " promoted " : " ") + PieceType;
    }
    /// <summary>Gets user friendly piece name with latin type for debug purposes</summary>
    /// <returns>e.g. White promoted R</returns>
    public string ToLatinString()
    {
      return (Owner == null ? "Ownerless" : Owner.Color.ToString()) + 
        (IsPromoted ? " promoted " : " ") + PieceType.Latin;
    }
  }
}