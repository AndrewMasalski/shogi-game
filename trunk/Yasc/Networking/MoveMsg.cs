using System;

namespace Yasc.Networking
{
  [Serializable]
  public class MoveMsg
  {
    public string Move { get; private set; }
    public DateTime TimeStamp { get; private set; }

    public MoveMsg(string move, DateTime timeStamp)
    {
      Move = move;
      TimeStamp = timeStamp;
    }
    public MoveMsg(string move)
    {
      Move = move;
      TimeStamp = DateTime.Now;
    }
  }
}