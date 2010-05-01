using System.Collections.Generic;
using Yasc.ShogiCore;

namespace Yasc.Networking.Interfaces
{
  public interface IServerGame
  {
    ISpectatorController Watch();
    IServerUser Invitor { get; }
    IServerUser Invitee { get; }
    PieceColor InviteeColor { get; }
    IEnumerable<ISpectatorController> Spectators { get; }
  }
}