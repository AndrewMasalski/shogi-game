using System;
using Yasc.ShogiCore.Primitives;

namespace Yasc.ShogiCore.Snapshots
{
  /// <summary>Lightweight representation of drop move (as opposing to <see cref="UsualMove"/></summary>
  [Serializable]
  public sealed class DropMove : Move
  {
    /// <summary>Type and color of piece being dropped</summary>
    public IColoredPiece Piece { get; private set; }
    /// <summary>Position on the board to drop piece to</summary>
    public Position To { get; private set; }

    public override MoveType MoveType
    {
      get { return MoveType.Drop; }
    }

    /// <summary>Gets the color of player who made the move</summary>
    public override PieceColor Who
    {
      get { return Piece.Color; }
    }

    internal override RulesViolation Validate()
    {
      return BoardSnapshotBefore.ValidateDropMove(this);
    }

    /// <summary>Type of piece being dropped</summary>
    public IPieceType PieceType { get { return Piece.PieceType; } }

    /// <summary>ctor</summary>
    public DropMove(BoardSnapshot boardSnapshot, IColoredPiece coloredPiece, Position to)
      : base(boardSnapshot)
    {
      // TODO: Change signature to match Board.GetDropMove()
      Piece = coloredPiece;
      To = to;
    }

    internal override void Apply(BoardSnapshot board)
    {
      var handCollection = board.GetHandCollection(board.SideOnMove);
      handCollection.Remove(Piece.PieceType);
      board.SetPiece(To, Piece);
    }

    /// <summary>Returns user friendly move representation (latin)</summary>
    public override string ToString()
    {
      return PieceType.Latin + "'" + To;
    }
  }
}