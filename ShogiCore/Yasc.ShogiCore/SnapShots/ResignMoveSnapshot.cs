using System;
using Yasc.ShogiCore.Moves;
using Yasc.ShogiCore.Snapshots;

namespace Yasc.ShogiCore.SnapShots
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

    public override PieceColor GetColor(BoardSnapshot snapshot)
    {
      return _who;
    }

    protected override MoveBase AsRealMoveCore(Board board)
    {
      return new ResignMove(board, board[_who]);
    }
  }
}