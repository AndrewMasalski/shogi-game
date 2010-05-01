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
    /// <summary>Engine proposes to resign in the given situation</summary>
    public bool Resign { get; private set; }

    /// <summary>Creates an instance of <see cref="BestMoveEventArgs"/> 
    ///   with <see cref="Resign"/> = false</summary>
    internal BestMoveEventArgs(string move, string ponder)
    {
      Move = move;
      Ponder = ponder;
    }
    /// <summary>Creates an instance of <see cref="BestMoveEventArgs"/> 
    ///   with <see cref="Resign"/> = true</summary>
    internal BestMoveEventArgs()
    {
      Resign = true;
    }
  }
}