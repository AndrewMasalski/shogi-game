using System;
using Yasc.ShogiCore.Utils;

namespace Yasc.Networking
{
  public interface IPlayerGameController
  {
    PieceColor MyColor { get; }

    void Move(string move);
    void Say(string move);

    event Action<string> OpponentMadeMove;
    event Action<string> OpponentSaidSomething;
  }
}