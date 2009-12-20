using System;
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

    internal Cell(Board board, Position position)
    {
      Board = board;
      Position = position;
    }

    public void SetPiece(Piece piece, Player owner)
    {
      if (piece == null) throw new ArgumentNullException("piece");
      if (owner == null) throw new ArgumentNullException("owner");
      if (piece.Owner != null)
      {
        throw new InvalidOperationException(
          "Piece can't be in two places at the same time. " +
          "First return it to the PieceSet, then try to add it to the hand");
      }

      piece.Owner = owner;

      Board.PieceSet.Pop(piece);
      Piece = piece;
      RaisePropertyChanged("Piece");
    }
    public void SetPiece(Piece piece)
    {
      if (Piece == piece) return;

      var player = piece.Owner;
      if (player == null) throw new Exception();
      player.Board.PieceSet.Push(piece);
      SetPiece(piece, player);
    }
    public Piece ResetPiece()
    {
      if (Piece == null) return null;
      var old = Piece;
      Piece = null;
      Board.PieceSet.Push(old);
      RaisePropertyChanged("Piece");
      return old;
    }
  }
}