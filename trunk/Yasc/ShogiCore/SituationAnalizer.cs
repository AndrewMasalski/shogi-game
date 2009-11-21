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
      foreach (var move in GetAllValidUsualMovesWithoutCheck3(board, Opponent(color)))
        if (move.To == king)
          return true;
      return false;
    }
    public static IEnumerable<MoveSnapshotBase> GetAllValidMoves(BoardSnapshot board, PieceColor color)
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
          foreach (var move in PromotionAnalyser.DuplicateForPromoting(board, GetAllValidUsualMoves(board, p)))
            yield return move;
    }
    private static IEnumerable<UsualMoveSnapshot> GetAllValidUsualMovesWithoutCheck3(BoardSnapshot board, PieceColor color)
    {
      foreach (var p in Position.OnBoard)
        if (board[p] != null && board[p].Color == color)
          foreach (var move in GetAllValidUsualMovesWithoutCheck3(board, p))
            yield return move;
    }
    private static IEnumerable<UsualMoveSnapshot> GetAllValidUsualMovesWithoutCheck3(BoardSnapshot board, Position f)
    {
      return from p in new AvailableUsualMovesAnalyser(board, f)
             select new UsualMoveSnapshot(f, p, false);
    }
    private static IEnumerable<UsualMoveSnapshot> GetAllValidUsualMoves(BoardSnapshot board, Position f)
    {
      return from p in new AvailableUsualMovesAnalyser(board, f)
             let move = new UsualMoveSnapshot(f, p, false)
             where UsualMovesValidator.Check3(board, move) == null
             select move;
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
    private static bool DoesntHaveValidMoves(BoardSnapshot board, PieceColor color)
    {
      return GetAllValidMoves(board, color).FirstOrDefault() == null;
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