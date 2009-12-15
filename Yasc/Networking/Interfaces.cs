using System;
using System.Collections.Generic;
using Yasc.ShogiCore;

namespace Yasc.Networking
{
  public interface IServerSession
  {
    ShogiServer Server { get; }

    IServerUser[] Users { get; }
    IServerGame[] Games { get; }
    
    event Action<IInviteeTicket> InvitationReceived;
    void InvitePlay(IServerUser user, Action<IPlayerGameController> invitationAccepted);
  }

  public interface IInviteeTicket
  {
    IPlayerGameController Accept();
  }

  public interface IServerUser
  {
    string Name { get; }
    IServerGame CurrentGame { get; }
  }
  public interface IServerGame
  {
    ISpectatorController Watch();
    IServerUser Invitor { get; }
    IServerUser Invitee { get; }
    IEnumerable<ISpectatorController> Spectators { get; }

  }
  public interface IPlayerGameController
  {
    IServerGame Game { get; }
    PieceColor MyColor { get; }
    TimeSpan TimeLeft { get; }

    void Move(MoveMsg move);
    void Say(string move);

    Func<MoveMsg, DateTime> OpponentMadeMove { set; }
    event Action<string> OpponentSaidSomething;
  }

  public interface ISpectatorController
  {
    event Action<PieceColor, MoveMsg> PlayerMadeMove;
    event Action<PieceColor, string> PlayerSomeoneSaidSomething;
  }
}