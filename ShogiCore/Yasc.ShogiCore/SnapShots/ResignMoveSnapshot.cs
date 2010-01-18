using System;
using Yasc.ShogiCore.Moves;

namespace Yasc.ShogiCore.Snapshots
{
  /// <summary>Represents lightweight snapshot of resign move</summary>
  [Serializable]
  public sealed class ResignMoveSnapshot : MoveSnapshotBase
  {
    private readonly PieceColor _who;

    /// <summary>ctor</summary>
    public ResignMoveSnapshot(PieceColor who)
    {
      _who = who;
    }

    /// <summary>Override to determine color of the player the move belongs to</summary>
    public override PieceColor GetColor(BoardSnapshot snapshot)
    {
      return _who;
    }

    /// <summary>Override to convert snapshot to observable move</summary>
    protected override MoveBase AsRealMoveCore(Board board)
    {
      return new ResignMove(board, board[_who]);
    }
  }
}