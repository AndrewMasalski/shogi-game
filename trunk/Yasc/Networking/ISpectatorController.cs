using System;
using Yasc.ShogiCore.Utils;

namespace Yasc.Networking
{
  public interface ISpectatorController
  {
    event Action<PieceColor, string> PlayerMadeMove;
    event Action<PieceColor, string> PlayerSomeoneSaidSomething;
  }
}