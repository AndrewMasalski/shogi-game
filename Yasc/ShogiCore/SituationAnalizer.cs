using System.Collections.Generic;
using Yasc.ShogiCore.Moves;
using Yasc.ShogiCore.Moves.Validation;
using Yasc.ShogiCore.Utils;
using System.Linq;

namespace Yasc.ShogiCore
{
  public static class SituationAnalizer
  {
    public static bool IsMateFor(BoardSnapshot board, PieceColor color)
    {
      return IsCheckFor(board, color) && DoesntHaveValidMoves(board, color);
    }
    public static bool IsCheckFor(BoardSnapshot board, PieceColor color)
    {
      var king = FindTheKing(board, color);
      if (king == null) return false;
      foreach (var move in GetAllValidUsualMoves(board, Opponent(color)))
        if (move.To == king)
          return true;
      return false;
    }

    private static bool DoesntHaveValidMoves(BoardSnapshot board, PieceColor color)
    {
      return GetAllValidMoves(board, color).FirstOrDefault() == null;
    }
    private static IEnumerable<MoveSnapshotBase> GetAllValidMoves(BoardSnapshot board, PieceColor color)
    {
      foreach (var move in GetAllValidUsualMoves(board, color))
        yield return move;
      foreach (var move in GetAllValidDropMoves(board, color))
        yield return move;
    }
    private static IEnumerable<UsualMoveSnapshot> GetAllValidUsualMoves(BoardSnapshot board, PieceColor color)
    {
      foreach (var p in Position.OnBoard)
        if (board[p] != null && board[p].Color == color)
          foreach (var move in GetAllValidUsualMoves(board, p))
            yield return move;
    }
    private static IEnumerable<MoveSnapshotBase> GetAllValidDropMoves(BoardSnapshot board, PieceColor color)
    {
      foreach (var piece in board.Hand(color).Distinct())
        foreach (var p in Position.OnBoard)
        {
          var move = new DropMoveSnapshot(piece, p);
          if (DropMoveValidator.GetError(board, move) == null)
            yield return move;
        }
    }
    private static IEnumerable<UsualMoveSnapshot> GetAllValidUsualMoves(BoardSnapshot board, Position from)
    {
      foreach (var move in GetAllUsualMoves(from))
        if (UsualMovesValidator.GetError(board, move) == null)
          yield return move;
    }
    private static IEnumerable<UsualMoveSnapshot> GetAllUsualMoves(Position from)
    {
      foreach (var p in Position.OnBoard)
      {
        yield return new UsualMoveSnapshot(from, p, false);
        yield return new UsualMoveSnapshot(from, p, true);
      }
    }
    private static Position? FindTheKing(BoardSnapshot board, PieceColor color)
    {
      foreach (var p in Position.OnBoard)
        if (board[p] != null)
          if (board[p].Color == color)
            if (board[p].Type == PieceType.玉)
              return p;

      return null;
    }
    private static PieceColor Opponent(PieceColor color)
    {
      return color == PieceColor.White ? PieceColor.Black : PieceColor.White;
    }
  }
}