using System;
using Yasc.Networking.Utils;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Primitives;

namespace Yasc.Networking.Interfaces
{
  public interface IPlayerGameController
  {
    IServerGame Game { get; }
    PieceColor MyColor { get; }
    TimeSpan TimeLeft { get; }

    void Move(MoveMsg move);
    void Say(string move);
    void UndoLastMove();

    Func<MoveMsg, DateTime> OpponentMadeMove { set; }
    event Action<string> OpponentSaidSomething;
    event EventHandler OpponentTakesBack;
  }
}