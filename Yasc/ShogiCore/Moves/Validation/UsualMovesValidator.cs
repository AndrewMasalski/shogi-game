using System.Diagnostics;
using Yasc.ShogiCore.Utils;
using System.Linq;

namespace Yasc.ShogiCore.Moves.Validation
{
  public static class UsualMovesValidator
  {
    public static string GetError(BoardSnapshot board, UsualMoveSnapshot move)
    {
      bool isPromotion = move.IsPromoting;
      
      var piece = board[move.From];
      if (piece == null) return "No piece at " + move.From;
      if (move.From == move.To) return "You can't move from " + move.From + " to " + move.To;
      if (piece.Color != board.OneWhoMoves) return "It's " + board.OneWhoMoves + "'s move now";

      if (board[move.To] != null)
      {
        if (piece.Color == board[move.To].Color) return "Cant take piece of the same color";
      }

      var analyser = new AvailableMovesAnalyser(board, move.From);
      if (!analyser.Contains(move.To))
      {
        return piece.Type + " doesn't move this way";
      }

      if (piece.Type == PieceType.歩 || piece.Type == PieceType.香)
      {
        if (piece.HowFarFromTheLastLine(move.To) == 0)
        {
          if (!isPromotion)
          {
            return piece.Type + " can't move to the last line without promotion";
          }
        }
      }

      if (piece.Type == PieceType.桂)
      {
        if (piece.HowFarFromTheLastLine(move.To) < 2)
        {
          if (!isPromotion)
          {
            return "桂 can't move to the last two lines without promotion";
          }
        }
      }

      if (isPromotion)
      {
        var error = Promotion(move.From, move.To, piece);
        if (error != null)
          return error;
      }

      if (new BoardSnapshot(board, move).IsCheckFor(board.OneWhoMoves))
      {
        return "If you made this move your king would be taken on the next move";
      }

      return null;
    }

    private static string Promotion(Position from, Position to, PieceSnapshot piece)
    {
      if (piece.IsPromoted)
        return "Can't promote piece that is already promoted";
      
      if (!piece.Type.CanPromote)
        return "Can't promote " + piece.Type;

      if (!piece.IsThatPromitionZoneFor(from))
        if (!piece.IsThatPromitionZoneFor(to))
          return "Can't promote " + piece.Type + 
            " when move from line " + from.Line + " to line " + to.Line;

      return null;
    }
  }
}