using System;
using System.Collections.Generic;
using System.Threading;
using Yasc.Networking;
using Yasc.ShogiCore;

namespace Yasc.AI
{
  public abstract class AiControllerBase : IPlayerGameController
  {
    private Func<MoveMsg, DateTime> _compMadeMove;
    private readonly SynchronizationContext _synch = SynchronizationContext.Current;

    #region Implementation of IPlayerGameController

    IServerGame IPlayerGameController.Game
    {
      get { return new AiGame(); }
    }
    PieceColor IPlayerGameController.MyColor
    {
      get { return PieceColor.White; }
    }
    TimeSpan IPlayerGameController.TimeLeft
    {
      get { return TimeSpan.FromHours(1); }
    }
    void IPlayerGameController.Move(MoveMsg move)
    {
      // Human moved
      OnHumanMoved(move.Move);
    }
    void IPlayerGameController.Say(string move)
    {
      // Comp will ignore it
    }
    Func<MoveMsg, DateTime> IPlayerGameController.OpponentMadeMove
    {
      set { _compMadeMove = value;  }
    }
    event Action<string> IPlayerGameController.OpponentSaidSomething
    {
      add { }
      remove { }
    }

    private class AiGame : IServerGame
    {
      public ISpectatorController Watch()
      {
        throw new InvalidOperationException();
      }

      public IServerUser Invitor
      {
        get { return new Human(this); }
      }
      public IServerUser Invitee
      {
        get { return new Comp(this); }
      }

      public IEnumerable<ISpectatorController> Spectators
      {
        get { return new ISpectatorController[0]; }
      }
    }
    private class Comp : IServerUser
    {
      private readonly IServerGame _currentGame;

      public Comp(IServerGame currentGame)
      {
        _currentGame = currentGame;
      }

      public string Name
      {
        get { return "Computer"; }
      }

      public IServerGame CurrentGame
      {
        get { return _currentGame; }
      }
    }
    private class Human : IServerUser
    {
      private readonly IServerGame _currentGame;

      public Human(IServerGame currentGame)
      {
        _currentGame = currentGame;
      }

      public string Name
      {
        get { return "You"; }
      }

      public IServerGame CurrentGame
      {
        get { return _currentGame; }
      }
    }

    #endregion

    protected abstract void OnHumanMoved(string move);
    protected void Move(string move)
    {
      if (_synch == null)
      {
        _compMadeMove(new MoveMsg(move, DateTime.Now));
      }
      else
      {
        _synch.Post(state => _compMadeMove(new MoveMsg(move, DateTime.Now)), null);
      }
    }
  }
}