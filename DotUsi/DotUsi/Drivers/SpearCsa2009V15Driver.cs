using Yasc.DotUsi.Process;

namespace Yasc.DotUsi.Drivers
{
  ///<summary>Driver for Spear CSA 2009 v.1.5 </summary>
  public class SpearCsa2009V15Driver : UsiDriverBase
  {
    private bool _gameMode;
    
    /// <summary>ctor</summary>
    /// <param name="process">Raw process to decorate</param>
    public SpearCsa2009V15Driver(IUsiProcess process) 
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
  }
}