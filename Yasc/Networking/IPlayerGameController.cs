using System;
using Yasc.ShogiCore.Utils;

namespace Yasc.Networking
{
  public interface IPlayerGameController
  {
    PieceColor MyColor { get; }
    TimeSpan TimeLeft { get; }

    void Move(MoveMsg move);
    void Say(string move);

    event Action<MoveMsg> OpponentMadeMove;
    event Action<string> OpponentSaidSomething;
  }
}