using Yasc.ShogiCore.Primitives;
using Yasc.ShogiCore.Snapshots;

namespace Yasc.ShogiCore.Core
{
  /// <summary>Represents drop move (as opposing to <see cref="UsualMove"/>)</summary>
  public sealed class DropMove : MoveBase
  {
    private string _errorMessage;

    /// <summary>Droping piece type</summary>
    public PieceType PieceType { get; private set; }
    /// <summary>Position to drop piece to</summary>
    public Position To { get; private set; }

    private DropMove(Board board, PieceType pieceType, Position to, Player who) 
      : base(board, who)
    {
      PieceType = pieceType;
      To = to;
    }
    /// <summary>Creates an instance of <see cref="DropMove"/> 
    ///   from type and position and validates it immediately</summary>
    public static DropMove Create(Board board, PieceType pieceType, Position to, Player who)
    {
      return new DropMove(board, pieceType, to, who);
    }
    /// <summary>Creates an instance of <see cref="DropMove"/> 
    ///   from snapshot and validates it immediately</summary>
    public static DropMove Create(Board board, DropMoveSnapshot snapshot)
    {
      return new DropMove(board, snapshot.Piece.PieceType, 
        snapshot.To, board[snapshot.Piece.Color]);
    }

    public override string ErrorMessage
    {
      get
      {
        return _errorMessage = _errorMessage ??  BoardSnapshot.
          ValidateDropMove(new DropMoveSnapshot(PieceType, Who.Color, To));
      }
    }

    /// <summary>Applies move to the <see cref="MoveBase.Board"/></summary>
    protected internal override void Make()
    {
      var piece = Who.Hand.GetByType(PieceType);
      Who.Hand.Remove(piece);
      Board.SetPiece(piece, Who, To);
    }

    /// <summary>Gets snapshot of the <see cref="DropMove"/></summary>
    public override MoveSnapshotBase Snapshot()
    {
      return new DropMoveSnapshot(PieceType, Who.Color, To);
    }

    /// <summary>Returns user friendly move representation (latin)</summary>
    public override string ToString()
    {
      return PieceType.Latin + "'" + To;
    }
  }
}