using Yasc.ShogiCore.Utils;

namespace Yasc.ShogiCore.Moves.Validation
{
  public static class DropMoveValidator
  {
    public static string GetError(BoardSnapshot board, DropMoveSnapshot move)
    {
      if (move.Piece.Color != board.OneWhoMoves) return "It's " + board.OneWhoMoves + "'s move now";
      if (!board.Hand(board.OneWhoMoves).Contains(move.Piece)) return "Player doesn't have this piece in hand";
      if (board[move.To] != null) return "Can't take piece with drop move";
      
      if (move.Piece.Type == PieceType.歩 || move.Piece.Type == PieceType.香)
      {
        if (move.Piece.HowFarFromTheLastLine(move.To) == 0)
        {
          return "Can't drop " + move.Piece.Type + " to the last line";
        }
      }

      if (move.Piece.Type == PieceType.桂)
      {
        if (move.Piece.HowFarFromTheLastLine(move.To) < 2)
        {
          return "Can't drop 桂 to the last two lines";
        }
      }

      if (move.Piece.Type == PieceType.歩)
      {
        if (IsTherePawnOnThisColumn(board, board.OneWhoMoves, move.To.Y))
        {
          return "Can't drop 歩 to the column " + move.To.Column + " because it already has one 歩";
        }
      }

      var newPosition = new BoardSnapshot(board, move);
      if (move.Piece.Type == PieceType.歩 && newPosition.IsMateFor(Opponent(board.OneWhoMoves)))
      {
        return "Can't drop 歩 to mate the opponent";
      }
      if (newPosition.IsCheckFor(board.OneWhoMoves))
      {
        return "If you made this move your king would be taken on the next move";
      }
      return null;
    }

    private static PieceColor Opponent(PieceColor color)
    {
      return color == PieceColor.White ? PieceColor.Black : PieceColor.White;
    }

    private static bool IsTherePawnOnThisColumn(BoardSnapshot board, PieceColor color, int column)
    {
      for (int i = 0; i < 9; i++)
        if (board[i, column].Type == PieceType.歩)
          if (board[i, column].Color == color)
            return true;

      return false;
    }
  }
}