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
    /// <summary>Board the cell belongs to</summary>
    public Board Board { get; set; }
    /// <summary>Position of the cell on the board</summary>
    public Position Position { get; private set; }
    /// <summary>Piece in the cell</summary>
    public Piece Piece { get; private set; }

    internal Cell(Board board, Position position)
    {
      Board = board;
      Position = position;
    }

    /// <summary>Places the piece into the cell</summary>
    /// <remarks>Method takes ownerless piece and places it into the cell</remarks>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="piece"/> or <paramref name="owner"/> is null
    /// </exception>
    /// <exception cref="InvalidOperationException">The piece is not ownerless</exception>
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
    /// <summary>Places the piece into the cell</summary>
    /// <remarks>Method takes piece and places it into the cell</remarks>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="piece"/> is null
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">the piece has no owner</exception>
    public void SetPiece(Piece piece)
    {
      if (piece == null) throw new ArgumentNullException("piece");
      if (Piece == piece) return;

      var player = piece.Owner;
      if (player == null) 
        throw new ArgumentOutOfRangeException("piece", "must have owner");
      player.Board.PieceSet.Push(piece);
      SetPiece(piece, player);
    }
    /// <summary>Removes the piece from the cell to the piece set</summary>
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