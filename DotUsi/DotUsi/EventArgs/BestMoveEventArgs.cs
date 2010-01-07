using System;

namespace DotUsi
{
  /// <summary>Holds the data engine sends when the best move is found</summary>
  public class BestMoveEventArgs : EventArgs
  {
    /// <summary>The best move</summary>
    public string Move { get; private set; }
    /// <summary>The move engine would like to ponder on or null if engine is lazy</summary>
    public string Ponder { get; private set; }

    /// <summary>ctor</summary>
    internal BestMoveEventArgs(string move, string ponder)
    {
      Move = move;
      Ponder = ponder;
    }
  }
}