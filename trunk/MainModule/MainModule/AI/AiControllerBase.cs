using System;
using System.Collections.Generic;
using System.Threading;
using Yasc.Networking.Interfaces;
using Yasc.Networking.Utils;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Primitives;

namespace MainModule.AI
{
  public abstract class AiControllerBase : IPlayerGameController
  {
    private Func<MoveMsg, DateTime> _compMadeMove;
    private readonly SynchronizationContext _synch = SynchronizationContext.Current;

    #region Implementation of IPlayerGameController

    public abstract void UndoLastMove();

    public IServerGame Game
    {
      get { return new AiGame(); }
    }

    public PieceColor MyColor
    {
      get { return PieceColor.White; }
    }

    public TimeSpan TimeLeft
    {
      get { return TimeSpan.FromHours(1); }
    }

    public void Move(MoveMsg move)
    {
      // Human moved
      ThreadPool.QueueUserWorkItem(s => OnHumanMoved(move.Move));
    }

    public void Say(string move)
    {
      // Comp will ignore it
    }

    public Func<MoveMsg, DateTime> OpponentMadeMove
    {
      set { _compMadeMove = value;  }
    }

    public event Action<string> OpponentSaidSomething
    {
      add { }
      remove { }
    }

    public event EventHandler OpponentTakesBack;

    protected void InvokeOpponentTakesBack(EventArgs e)
    {
      var handler = OpponentTakesBack;
      if (handler != null) handler(this, e);
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

      public PieceColor InviteeColor
      {
        get { return PieceColor.White; }
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

    protected abstract void OnHumanMoved(string hisMove);
    protected void Move(string transcript)
    {
      if (_synch == null)
      {
        _compMadeMove(new MoveMsg(transcript, DateTime.Now));
      }
      else
      {
        _synch.Post(state => _compMadeMove(new MoveMsg(transcript, DateTime.Now)), null);
      }
    }
  }
}