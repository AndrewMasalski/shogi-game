using System.Collections.Generic;
using DotUsi.Options;
using DotUsi.Options.Base;
using DotUsi.Process;

namespace DotUsi.Drivers
{
  ///<summary>Driver for Spear CSA 2008 v.1.4 </summary>
  public class SpearCsa2008V14Driver : UsiDriverBase, IEngineHook
  {
    private bool _gameMode;
    private UsiEngine _engine;

    /// <summary>ctor</summary>
    /// <param name="process">Raw process to decorate</param>
    public SpearCsa2008V14Driver(IUsiProcess process) 
      : base(process)
    {
    }

    /// <summary>Override to change behaviour</summary>
    public override void WriteLine(string text)
    {
      if (text == "usinewgame")
      {
        _gameMode = true;
      }
      else if (text == "isready" && _gameMode)
      {
        OnOutputDataReceived(new LineReceivedEventArgs("readyok"));
      }
      base.WriteLine(text);
    }

    void IEngineHook.SetEngine(UsiEngine engine)
    {
      _engine = engine;
    }

    IEnumerable<UsiOptionBase> IEngineHook.GetImplicitOptions()
    {
      yield return new CheckOption(_engine, "USI_Ponder", true);
      yield return new SpinOption(_engine, "USI_Hash", true, 32);
    }
  }
}