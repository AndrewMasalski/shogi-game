using MvvmFoundation.Wpf;

namespace Yasc.ShogiCore
{
  /// <summary>Cell is a place on a board where piece may appear. 
  ///   Shogi board is consist of 9x9 cells.</summary>
  /// <remarks>This class allows the board to be collection of cells,
  ///   which dramatically simplifies GUI binding</remarks>
  public class Cell : ObservableObject
  {
    public Board Board { get; set; }
    public Position Position { get; private set; }
    public Piece Piece { get; private set; }

    public void SetPiece(Piece piece, Player owner)
    {
      piece.Owner = owner;

      if (Piece == piece) return;
      Board.PieceSet.Take(piece);
      Piece = piece;
      RaisePropertyChanged("Piece");
    }
    public void SetPiece(Piece piece)
    {
      SetPiece(piece, piece.Owner);
    }
    public Piece ResetPiece()
    {
      if (Piece == null) return null;
      var old = Piece;
      Piece = null;
      Board.PieceSet.Return(old);
      RaisePropertyChanged("Piece");
      return old;
    }

    internal Cell(Board board, Position position)
    {
      Board = board;
      Position = position;
    }
  }
}