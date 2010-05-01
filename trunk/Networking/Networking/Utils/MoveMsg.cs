using System;

namespace Yasc.Networking.Utils
{
  [Serializable]
  public class MoveMsg
  {
    public string Move { get; private set; }
    public DateTime Timestamp { get; private set; }

    public MoveMsg(string move, DateTime timestamp)
    {
      Move = move;
      Timestamp = timestamp;
    }
    public MoveMsg(string move)
    {
      Move = move;
      Timestamp = DateTime.Now;
    }
  }
}