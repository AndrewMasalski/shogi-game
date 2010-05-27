using System;
using Yasc.ShogiCore.Primitives;

namespace Yasc.ShogiCore.Snapshots
{
  /// <summary>Lightweight representation of the usual move (as opposing to <see cref="DropMove"/></summary>
  [Serializable]
  public sealed class UsualMove : Move
  {
    private readonly PieceColor _who;

    /// <summary>Position on the board move originates from</summary>
    public Position From { get; private set; }
    /// <summary>Position on the board piece is moving to</summary>
    public Position To { get; private set; }
    /// <summary>Indicates whether the move is promoting</summary>
    public bool IsPromoting { get; private set; }
    /// <summary>Gets the color of player who made the move</summary>
    public override PieceColor Who
    {
      get { return _who; }
    }

    /// <summary>ctor</summary>
    public UsualMove(PieceColor who, Position from, Position to, bool isPromoting)
    {
      _who = who;
      From = from;
      To = to;
      IsPromoting = isPromoting;
    }
    /// <summary>Gets user friendly transcription of the move (latin)</summary>
    public override string ToString()
    {
      return From + "-" + To + (IsPromoting ? "+" : "");
    }
  }
}