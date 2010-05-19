using System;
using Yasc.Networking.Interfaces;
using Yasc.Networking.Utils;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Primitives;

namespace Yasc.Networking
{
  public partial class ShogiServer
  {
    private class PlayerGameController : MarshalByRefObject, IPlayerGameController
    {
      private readonly ServerGame _game;
      private DateTime? _gotLastOpponentMoveAt;

      public TimeSpan TimeLeft { get; private set; }
      public IServerGame Game
      {
        get { return _game; }
      }
      public PieceColor MyColor { get; private set; }
      public void Move(MoveMsg move)
      {
        _game.Move(this, move);
        if (_gotLastOpponentMoveAt != null)
        {
          TimeLeft -= move.Timestamp - (DateTime)_gotLastOpponentMoveAt;
        }
      }
      public void Say(string text)
      {
        _game.Say(this, text);
      }

      public void UndoLastMove()
      {
        _game.UndoLastMove(this);
      }

      public Func<MoveMsg, DateTime> OpponentMadeMove { private get; set; }
      public event Action<string> OpponentSaidSomething;
      public PlayerGameController(ServerGame game, PieceColor color)
      {
        _game = game;
        MyColor = color;
        TimeLeft = TimeSpan.FromMinutes(5);
      }


      public void InvokeOpponentMadeMove(MoveMsg move)
      {
        var handler = OpponentMadeMove;
        if (handler != null)
        {
          _gotLastOpponentMoveAt = handler(move);
        }
      }
      public void InvokeOpponentSaidSomething(string obj)
      {
        var something = OpponentSaidSomething;
        if (something != null) something(obj);
      }

      public event EventHandler OpponentTakesBack;

      public void InvokeOpponentTakesBack()
      {
        var handler = OpponentTakesBack;
        if (handler != null) handler(this, EventArgs.Empty);
      }
    }
  }
}