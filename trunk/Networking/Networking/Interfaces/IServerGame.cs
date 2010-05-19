using System.Collections.Generic;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Primitives;

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