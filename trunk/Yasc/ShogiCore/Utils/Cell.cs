using MvvmFoundation.Wpf;

namespace Yasc.ShogiCore.Utils
{
  /// <summary>Cell is a place on a board where piece may appear. 
  ///   Shogi board is consist of 9x9 cells.</summary>
  /// <remarks>This class is invented to make board to be collection of cells. 
  ///   That drammaticaly simplifies GUI binding</remarks>
  public class Cell : ObservableObject
  {
    private Piece _piece;

    public Position Position { get; private set; }
    public bool IsInPromotionZone
    {
      get { return Position.Y < 3 || Position.Y >= 6; }
    }
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

    public Cell(Position position)
    {
      Position = position;
    }
  }
}