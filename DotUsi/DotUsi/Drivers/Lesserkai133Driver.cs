namespace DotUsi
{
  public class Lesserkai133Driver : UsiDriverBase
  {
    private bool _gameMode;

    public Lesserkai133Driver(IUsiProcess process) 
      : base(process)
    {
    }

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