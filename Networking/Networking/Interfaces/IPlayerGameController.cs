using System;
using Yasc.ShogiCore;

namespace Yasc.Networking
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