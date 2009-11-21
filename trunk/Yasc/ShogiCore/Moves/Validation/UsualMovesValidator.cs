using Yasc.ShogiCore.Utils;
using System.Linq;

namespace Yasc.ShogiCore.Moves.Validation
{
  public static class UsualMovesValidator
  {
    /// <summary>Checks everything</summary>
    /// <remarks><para>Check1 = Check2 + fool checks like is there piece 
    ///   in "from" cell, does it have appropriate color, etc.</para>
    ///   <para>If move is initiated from sophisticated GUI you might
    ///      omit this check and start from Check2</para>
    ///   <para>Check1 can be omitted if you know that: <list type="number">
    ///   <item>there is a piece in "from" cell. </item>
    ///   <item>to != from </item>
    ///   <item>piece.Color == board.OneWhoMoves </item>
    ///   <item>piece.Color != board[move.To].Color </item></list></para>
    /// </remarks>
    public static string Check1(BoardSnapshot board, UsualMoveSnapshot move)
    {
      var piece = board[move.From];

      if (piece == null) 
        return "No piece at " + move.From;

      if (move.From == move.To) 
        return "You can't move from " + move.From + " to " + move.To;

      if (piece.Color != board.OneWhoMoves) 
        return "It's " + board.OneWhoMoves + "'s move now";

      if (board[move.To] != null)
        if (piece.Color == board[move.To].Color) 
          return "Cant take piece of the same color";

      var analyser = new AvailableUsualMovesAnalyser(board, move.From);
      if (!analyser.Contains(move.To))
      {
        return board[move.From].Type + " doesn't move this way";
      }

      return Check3(board, move);
    }
    /// <summary>Checks just exceptions like moves to check</summary>
    /// <reremarks>
    ///   <para>If move is proposed with <see cref="AvailableUsualMovesAnalyser"/>
    ///     in conjunction with <see cref="PromotionAnalyser"/> you can
    ///     omit other checks but this</para>
    /// </reremarks>
    public static string Check3(BoardSnapshot board, UsualMoveSnapshot move)
    {
      if (move.IsPromoting)
      {
        var error = IsPromotionAllowed(board[move.From], move.From, move.To);
        if (error != null)
          return error;
      }
      else
      {
        var error = IsPromotionMandatory(board[move.From], move.To);
        if (error != null)
          return error;
      }


      var snapshot = new BoardSnapshot(board, move);
      return !snapshot.IsCheckFor(move.GetColor(board)) ? null :
        "If you made this move your king would be taken on the next move";
    }

    public static string IsPromotionMandatory(PieceSnapshot piece, Position to)
    {
      if (piece.Type == PieceType.歩 || piece.Type == PieceType.香)
        if (piece.HowFarFromTheLastLine(to) == 0)
          return piece.Type + " cannot move to "
                 + "the last line without promotion";

      if (piece.Type == PieceType.桂)
        if (piece.HowFarFromTheLastLine(to) < 2)
          return "桂 cannot move to the last two lines without promotion";

      return null;
    }
    public static string IsPromotionAllowed(PieceSnapshot piece, Position from, Position to)
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