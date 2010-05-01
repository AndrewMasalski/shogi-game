using System;
using Yasc.ShogiCore.Snapshots;

namespace Yasc.ShogiCore.Moves
{
  /// <summary>Base class for the shogi move (usual and drop)</summary>
  public abstract class MoveBase
  {
    /// <summary>Board move belongs to</summary>
    public Board Board { get; private set; }
    /// <summary>Moment move is performed</summary>
    public DateTime Timestamp { get; private set; }
    /// <summary>Player performing the move</summary>
    public Player Who { get; private set; }
    /// <summary>Move sequential number within the game</summary>
    public int Number { get; private set; }
    /// <summary>Board snapshot before the move is done</summary>
    public BoardSnapshot BoardSnapshot { get; private set; }
    /// <summary>Indicates whether the move is valid</summary>
    public bool IsValid { get { return ErrorMessage == null; } }
    /// <summary>null if move is valid -or- explanation why it's not</summary>
    public string ErrorMessage { get; private set; }

    /// <summary>ctor</summary>
    protected MoveBase(Board board, Player who)
    {
      if (board == null) throw new ArgumentNullException("board");
      if (who == null) throw new ArgumentNullException("who");
      board.VerifyPlayerBelongs(who);
      
      Board = board;
      Timestamp = DateTime.Now;
      Who = who;
      Number = Board.History.Count + 1;
      BoardSnapshot = Board.CurrentSnapshot;
    }

    /// <summary>Override to apply move to the <see cref="MoveBase.Board"/></summary>
    protected internal abstract void Make();

    /// <summary>Validates the move</summary>
    protected void Validate()
    {
      ErrorMessage = GetValidationErrorMessage();
    }

    /// <summary>Override to get validation error message or null if move is valid</summary>
    protected abstract string GetValidationErrorMessage();

    /// <summary>Gets snapshot of the move</summary>
    public abstract MoveSnapshotBase Snapshot();
  }
}