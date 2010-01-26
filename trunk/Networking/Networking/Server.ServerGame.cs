using System;
using System.Collections.Generic;
using System.Linq;
using Yasc.ShogiCore;

namespace Yasc.Networking
{
  public partial class ShogiServer
  {
    private class ServerGame : MarshalByRefObject, IServerGame
    {
      private static readonly Random _rnd = new Random();
      private readonly ServerUser _invitor;
      private readonly ServerUser _invitee;
      private readonly List<SpectatorController> _spectators = new List<SpectatorController>();

      private ShogiServer _server;

      public PlayerGameController WhitePlayer { get; private set; }
      public PlayerGameController BlackPlayer { get; private set; }

      public IEnumerable<ISpectatorController> Spectators
      {
        get { return from s in _spectators select (ISpectatorController) s; }
      }
      public IServerUser Invitee
      {
        get { return _invitee; }
      }
      public IServerUser Invitor
      {
        get { return _invitor; }
      }
      public PieceColor InviteeColor { get; private set; }

      public ServerGame(ShogiServer server, ServerUser invitor, ServerUser invitee, PieceColor inviteeColor)
      {
        if (server == null) throw new ArgumentNullException("server");
        if (invitor == null) throw new ArgumentNullException("invitor");
        if (invitee == null) throw new ArgumentNullException("invitee");
        _server = server;
        _invitor = invitor;
        _invitee = invitee;
        InviteeColor = inviteeColor;

        WhitePlayer = new PlayerGameController(this, PieceColor.White);
        BlackPlayer = new PlayerGameController(this, PieceColor.Black);
      }

      public ISpectatorController Watch()
      {
        var spectator = new SpectatorController();
        _spectators.Add(spectator);
        return spectator;
      }

      private PlayerGameController Opponent(PlayerGameController controller)
      {
        return controller == WhitePlayer ? BlackPlayer : WhitePlayer;
      }

      public void Move(PlayerGameController controller, MoveMsg move)
      {
        Opponent(controller).InvokeOpponentMadeMove(move);

        foreach (var spectator in _spectators)
          spectator.InvokePlayerMadeMove(controller.MyColor, move);
      }

      public void Say(PlayerGameController controller, string text)
      {
        Opponent(controller).InvokeOpponentSaidSomething(text);

        foreach (var spectator in _spectators)
          spectator.InvokeSomeoneSaidSomething(controller.MyColor, text);
      }

      public void UndoLastMove(PlayerGameController controller)
      {
        Opponent(controller).InvokeOpponentTakesBack();
      }
    }
  }
}