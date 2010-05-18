using System;
using System.Collections.Generic;
using Yasc.ShogiCore.Snapshots;
using Yasc.Utils;

namespace Yasc.ShogiCore
{
  /// <summary>Describes one of the ways moves can be transcribed 
  ///   (like "1a-1i", "R'1a")</summary>
  public class FormalNotation : Singletone<FormalNotation>, INotation
  {
    public IEnumerable<MoveSnapshotBase> Parse(BoardSnapshot originalBoardState, string move)
    {
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
    public string ToString(BoardSnapshot originalBoardState, MoveSnapshotBase move)
    {
      throw new NotImplementedException();
    }
  }
}