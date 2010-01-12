using System;
using Yasc.ShogiCore;

namespace Yasc.Networking
{
  public interface ISpectatorController
  {
    event Action<PieceColor, MoveMsg> PlayerMadeMove;
    event Action<PieceColor, string> PlayerSomeoneSaidSomething;
  }
}