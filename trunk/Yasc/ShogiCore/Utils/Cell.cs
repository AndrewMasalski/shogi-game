using MvvmFoundation.Wpf;

namespace Yasc.ShogiCore.Utils
{
  /// <summary>Cell is a place on a board where piece may appear. 
  ///   Shogi board is consist of 9x9 cells.</summary>
  /// <remarks>This class allows the board to be collection of cells,
  ///   which dramatically simplifies GUI binding</remarks>
  public class Cell : ObservableObject
  {
    private Piece _piece;

    public Position Position { get; private set; }
    public Piece Piece
    {
      get { return _piece; }
      set
      {
        if (_piece == value) return;
        _piece = value;
        RaisePropertyChanged("Piece");
      }
    }

    internal Cell(Position position)
    {
      Position = position;
    }
  }
}