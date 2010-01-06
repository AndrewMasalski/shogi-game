using System;

namespace DotUsi
{
  public class BestMoveEventArgs : EventArgs
  {
    public string Move { get; private set; }
    public string Ponder { get; private set; }

    public BestMoveEventArgs(string move, string ponder)
    {
      Move = move;
      Ponder = ponder;
    }
  }
}