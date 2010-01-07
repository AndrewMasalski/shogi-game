using System;
using Yasc.ShogiCore;

namespace Yasc.Networking
{
  public partial class ShogiServer
  {
    private class SpectatorController : MarshalByRefObject, ISpectatorController
    {
      public event Action<PieceColor, MoveMsg> PlayerMadeMove;

      public void InvokePlayerMadeMove(PieceColor color, MoveMsg move)
      {
        var handler = PlayerMadeMove;
        if (handler != null) handler(color, move);
      }

      public event Action<PieceColor, string> PlayerSomeoneSaidSomething;

      public void InvokeSomeoneSaidSomething(PieceColor color, string move)
      {
        var handler = PlayerSomeoneSaidSomething;
        if (handler != null) handler(color, move);
      }
    }
  }
}