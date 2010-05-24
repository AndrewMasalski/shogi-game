using System;
using System.Collections.Generic;
using System.Linq;
using Yasc.ShogiCore.Primitives;

namespace Yasc.ShogiCore.Core
{
  /// <summary>Set of extension methods which makes work with 
  ///   grid handy althogh don't add any functionality</summary>
  public static class BoardExtensions
  {
    /// <summary>Gets the piece in the cell in the position -or- null if the cell is empty</summary>
    public static Piece GetPieceAt(this Board board, string position)
    {
      return board.GetPieceAt(Position.Parse(position));
    }

    #region ' Get/Make Move '

    /// <summary>Gets usual move on the board</summary>
    public static UsualMove GetUsualMove(this Board board, string from, string to, bool isPromoting)
    {
      return board.GetUsualMove(Position.Parse(from), Position.Parse(to), isPromoting);
    }
    /// <summary>Gets drop move on the board</summary>
    public static DropMove GetDropMove(this Board board, Piece piece, string to)
    {
      return board.GetDropMove(piece, Position.Parse(to));
    }
    /// <summary>Gets drop move on the board</summary>
    public static DropMove GetDropMove(this Board board, IPieceType piece, string to, Player who)
    {
      return board.GetDropMove(piece, Position.Parse(to), who);
    }
    /// <summary>Gets move on the board parsing it from trascript</summary>
    public static IEnumerable<MoveBase> GetMove(this Board board, string text, INotation notation)
    {
      if (notation == null) throw new ArgumentNullException("notation");
      return notation.Parse(board.CurrentSnapshot, text).Select(board.GetMove);
    }

    #endregion

    #region ' Set/Reset Piece '

    /// <summary>Set piece to the board cell</summary>
    public static void SetPiece(this Board board, Piece piece, string toPosition, Player forOwner)
    {
      board.SetPiece(piece, Position.Parse(toPosition), forOwner);
    }
    /// <summary>Set piece to the board cell</summary>
    public static void SetPiece(this Board board, Piece piece, string toPosition, PieceColor forOwner)
    {
      board.SetPiece(piece, Position.Parse(toPosition), forOwner);
    }
    /// <summary>Set piece to the board cell</summary>
    public static void SetPiece(this Board board, Piece piece, Position toPosition, PieceColor forOwner)
    {
      board.SetPiece(piece, toPosition, board.GetPlayer(forOwner));
    }
    /// <summary>Set piece to the board cell</summary>
    public static void SetPiece(this Board board, IPieceType pieceType, string toPosition, Player forOwner)
    {
      board.SetPiece(pieceType, Position.Parse(toPosition), forOwner);
    }
    /// <summary>Set piece to the board cell</summary>
    public static void SetPiece(this Board board, IPieceType pieceType, string toPosition, PieceColor forOwner)
    {
      board.SetPiece(pieceType, Position.Parse(toPosition), forOwner);
    }
    /// <summary>Set piece to the board cell</summary>
    public static void SetPiece(this Board board, IPieceType pieceType, Position toPosition, PieceColor forOwner)
    {
      board.SetPiece(pieceType, toPosition, board.GetPlayer(forOwner));
    }
    /// <summary>Removes the piece from the cell to the piece set</summary>
    public static Piece ResetPiece(this Board board, string position)
    {
      return board.ResetPiece(Position.Parse(position));
    }

    #endregion

    #region ' Analysis '

    /// <summary>Gets all valid usual moves available from the given position</summary>
    public static IEnumerable<UsualMove> GetAvailableMoves(this Board board, string fromPosition)
    {
      return board.GetAvailableMoves(Position.Parse(fromPosition));
    }

    #endregion
  }
}