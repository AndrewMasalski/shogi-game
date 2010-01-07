using System;
using System.Threading;
using DotUsi;

namespace UnitTests
{
  public static class EngineExtension
  {
    public static void SynchUsi(this UsiEngine engine)
    {
      using (var s = new UsiSychronizer(engine))
        s.Run();
    }
    public static void SynchNewGame(this UsiEngine engine)
    {
      using (var s = new NewGameSychronizer(engine))
        s.Run();
    }
    public static BestMoveEventArgs SynchGo(this UsiEngine engine, params UsiSearchModifier[] modifiers)
    {
      using (var s = new GoSychronizerBase(engine))
        return s.Run(modifiers);
    }

    #region ' EngineSychronizerBase '

    private abstract class EngineSychronizerBase : IDisposable
    {
      protected UsiEngine Engine { get; private set; }
      protected AutoResetEvent Event { get; private set; }

      protected EngineSychronizerBase(UsiEngine engine)
      {
        Event = new AutoResetEvent(false);
        Engine = engine;
      }

      public void Run()
      {
        Subscribe();
        RunCore();
        Event.WaitOne();
      }
      public void Dispose()
      {
        Unsubscribe();
        Event.Close();
      }

      protected abstract void Subscribe();
      protected abstract void Unsubscribe();
      protected abstract void RunCore();
    }

    #endregion

    #region ' NewGameSychronizer '

    private class NewGameSychronizer : EngineSychronizerBase
    {
      public NewGameSychronizer(UsiEngine engine) 
        : base(engine)
      {
      }

      protected override void Subscribe()
      {
        Engine.ReadyOK += EngineOnReadyOK;
      }
      protected override void Unsubscribe()
      {
        Engine.ReadyOK -= EngineOnReadyOK;
      }
      protected override void RunCore()
      {
        Engine.NewGame();
        Engine.IsReady();
      }
      private void EngineOnReadyOK(object sender, EventArgs args)
      {
        Event.Set();
      }
    }

    #endregion

    #region ' UsiSychronizer '

    private class UsiSychronizer : EngineSychronizerBase
    {
      public UsiSychronizer(UsiEngine engine) 
        : base(engine)
      {
      }

      protected override void Subscribe()
      {
        Engine.UsiOK += EngineOnUsiOK;
      }
      protected override void Unsubscribe()
      {
        Engine.UsiOK -= EngineOnUsiOK;
      }
      protected override void RunCore()
      {
        Engine.Usi();
      }
      private void EngineOnUsiOK(object sender, EventArgs args)
      {
        Event.Set();
      }
    }

    #endregion

    #region ' EngineSychronizerBase '

    private class GoSychronizerBase : IDisposable
    {
      private readonly UsiEngine _engine;
      private readonly AutoResetEvent _event;
      private BestMoveEventArgs _args;

      public GoSychronizerBase(UsiEngine engine)
      {
        _event = new AutoResetEvent(false);
        _engine = engine;
      }

      public BestMoveEventArgs Run(params UsiSearchModifier[] modifiers)
      {
        _engine.BestMove += EngineOnBestMove;
        _engine.Go(modifiers);
        _event.WaitOne();
        return _args;
      }
      public void Dispose()
      {
        _engine.BestMove -= EngineOnBestMove;
        _event.Close();
      }

      private void EngineOnBestMove(object sender, BestMoveEventArgs args)
      {
        _args = args;
        _event.Set();
      }
    }

    #endregion
  }
}