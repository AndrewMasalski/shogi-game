using Yasc.ShogiCore.Utils;

namespace Yasc.ShogiCore.Moves
{
  public class UsualMoveSnapshot : MoveSnapshotBase
  {
    public Position From { get; private set; }
    public Position To { get; private set; }
    public bool IsPromoting { get; private set; }

    public UsualMoveSnapshot(UsualMove move)
    {
      From = move.From;
      To = move.To;
      IsPromoting = move.IsPromoting;
    }

    public UsualMoveSnapshot(Position from, Position to, bool isPromoting)
    {
      From = from;
      To = to;
      IsPromoting = isPromoting;
    }
  }
}