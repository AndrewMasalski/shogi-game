using System;
using Yasc.Networking;
using Yasc.Networking.Interfaces;
using Yasc.Networking.Utils;
using Yasc.ShogiCore;

namespace Yasc.Utils
{
  public class GameTicket : IDisposable
  {
    private readonly IPlayerGameController _ticket;

    public PieceColor MyColor
    {
      get { return _ticket.MyColor; }
    }

    public IServerUser Me
    {
      get { return _ticket.Game.InviteeColor == MyColor ? _ticket.Game.Invitor : _ticket.Game.Invitee; }
    }
    public IServerUser Opponent
    {
      get { return _ticket.Game.InviteeColor != MyColor ? _ticket.Game.Invitor : _ticket.Game.Invitee; }
    }

    public GameTicket(IPlayerGameController ticket, Func<MoveMsg, DateTime> opponentMadeMoveCallback)
    {
      if (opponentMadeMoveCallback == null)
        throw new ArgumentNullException("opponentMadeMoveCallback");

      _ticket = ticket;
      _ticket.OpponentMadeMove = new FuncListener<MoveMsg, DateTime>(opponentMadeMoveCallback);
    }

    public void Move(MoveMsg msg)
    {
      _ticket.Move(msg);
    }

    public void Dispose()
    {
      if (_ticket is IDisposable)
      {
        ((IDisposable) _ticket).Dispose();
      }
    }

    public void UndoLastMove()
    {
      _ticket.UndoLastMove();
    }
  }
}