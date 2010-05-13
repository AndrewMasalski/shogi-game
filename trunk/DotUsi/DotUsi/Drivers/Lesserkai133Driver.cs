using Yasc.DotUsi.Process;

namespace Yasc.DotUsi.Drivers
{
  /// <summary>Driver for Lesserkai 1.3.3 engine</summary>
  public class Lesserkai133Driver : UsiDriverBase
  {
    private bool _gameMode;

    /// <summary>ctor</summary>
    /// <param name="process">Raw process to decorate</param>
    public Lesserkai133Driver(IUsiProcess process) 
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