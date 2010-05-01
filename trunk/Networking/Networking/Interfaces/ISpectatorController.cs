using System;
using Yasc.Networking.Utils;
using Yasc.ShogiCore;

namespace Yasc.Networking.Interfaces
{
  public interface ISpectatorController
  {
    event Action<PieceColor, MoveMsg> PlayerMadeMove;
    event Action<PieceColor, string> PlayerSomeoneSaidSomething;
  }
}