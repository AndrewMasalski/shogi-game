using System;
using System.Collections.Generic;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.Primitives;
using Yasc.ShogiCore.Snapshots;
using Yasc.Utils;

namespace Yasc.ShogiCore.Notations
{
  /// <summary>Describes one of the ways moves can be transcribed 
  ///   (like "1a-1i", "R'1a")</summary>
  public class FormalNotation : Singletone<FormalNotation>, INotation
  {
    /// <summary>Gets move on the board parsing it from transcript</summary>
    /// <param name="originalBoardState">State of the board before move</param>
    /// <param name="move">Move trancsript to parse</param>
    /// <returns>All moves which may be transcribed given way. 
    ///   Doesn't return null but be prepared to receive 0 moves.</returns>
    public IEnumerable<MoveSnapshotBase> Parse(BoardSnapshot originalBoardState, string move)
    {
      if (originalBoardState == null) throw new ArgumentNullException("originalBoardState");
      if (move == null) throw new ArgumentNullException("move");

      if (move == "resign")
      {
        yield return new ResignMoveSnapshot(originalBoardState.OneWhoMoves);
        yield break;
      }
      if (move.Contains("-"))
      {
        var elements = move.Split('-');
        var from = (Position)elements[0];
        var to = (Position)elements[1].Substring(0, 2);
        var modifier = elements[1].Length > 2 ? elements[1].Substring(2, 1) : null;

        var fromPiece = originalBoardState[from];
        if (fromPiece != null)
        {
          yield return new UsualMoveSnapshot(
            fromPiece.Color, from, to, modifier == "+");
        }
      }
      else
      {
        var elements = move.Split('\'');
        var piece = (PieceType)elements[0];
        var to = (Position)elements[1];

        yield return new DropMoveSnapshot(
          piece, originalBoardState.OneWhoMoves, to);
      }
    }
    /// <summary>Returns the transcript for a given move</summary>
    /// <param name="originalBoardState">State of the board before move</param>
    /// <param name="move">Move to trancsript</param>
    public string ToString(BoardSnapshot originalBoardState, MoveSnapshotBase move)
    {
      if (move == null) throw new ArgumentNullException("move");
      return move.ToString();
    }
  }
}