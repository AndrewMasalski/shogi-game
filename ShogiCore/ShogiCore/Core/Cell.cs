using Yasc.ShogiCore.Primitives;
using Yasc.Utils.Mvvm;

namespace Yasc.ShogiCore.Core
{
  /// <summary>Cell is a place on a board where piece may appear. 
  ///   Shogi board is consist of 9x9 cells.</summary>
  /// <remarks>This class allows the board to be collection of cells,
  ///   which dramatically simplifies GUI binding</remarks>
  public class Cell : ObservableObject
  {
    private Piece _piece;

    /// <summary>Gets the board cell belongs to</summary>
    public Board Owner { get; set; }
    /// <summary>Position of the cell on the board</summary>
    public Position Position { get; private set; }
    /// <summary>Piece in the cell</summary>
    public Piece Piece
    {
      get { return _piece; }
      internal set
      {
        if (_piece == value) return;
        _piece = value;
        Owner.CellPieceChanged();
        RaisePropertyChanged("Piece");
      }
    }

    internal Cell(Board owner, Position position)
    {
      Owner = owner;
      Position = position;
    }
  }
}