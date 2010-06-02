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

    public override MoveType MoveType
    {
      get { return MoveType.Usual; }
    }

    /// <summary>Gets the color of player who made the move</summary>
    public override PieceColor Who
    {
      get { return _who; }
    }

    internal override RulesViolation Validate()
    {
      return BoardSnapshotBefore.ValidateUsualMove(this);
    }

    /// <summary>ctor</summary>
    public UsualMove(BoardSnapshot boardSnapshot, PieceColor who, Position from, Position to, bool isPromoting)
      : base(boardSnapshot)
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

    internal override void Apply(BoardSnapshot board)
    {
      if (IsPromoting)
        board.SetPiece(From, board.GetPieceAt(From).Promoted);

      if (board.GetPieceAt(To) != null)
      {
        var handCollection = board.GetHandCollection(board.SideOnMove);
        handCollection.Add(board.GetPieceAt(To).PieceType.DemoteIfPossible());
      }
      board.SetPiece(To, board.GetPieceAt(From));
      board.SetPiece(From, null);
    }
  }
}