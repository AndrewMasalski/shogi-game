using MvvmFoundation.Wpf;

namespace Yasc.ShogiCore
{
  /// <summary>Cell is a place on a board where piece may appear. 
  ///   Shogi board is consist of 9x9 cells.</summary>
  /// <remarks>This class allows the board to be collection of cells,
  ///   which dramatically simplifies GUI binding</remarks>
  public class Cell : ObservableObject
  {
    public Position Position { get; private set; }
    public Piece Piece { get; private set; }

    public void SetPiece(Piece piece, Player owner)
    {
      piece.Owner = owner;

      if (Piece == piece) return;
      Piece = piece;
      RaisePropertyChanged("Piece");
    }
    public void SetPiece(Piece piece)
    {
      SetPiece(piece, piece.Owner);
    }
    public void ResetPiece()
    {
      if (Piece == null) return;
      Piece = null;
      RaisePropertyChanged("Piece");
    }

    internal Cell(Position position)
    {
      Position = position;
    }
  }
}