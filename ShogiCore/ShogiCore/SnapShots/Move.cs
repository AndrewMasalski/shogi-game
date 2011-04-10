using System;
using System.Diagnostics;
using Yasc.ShogiCore.Primitives;

namespace Yasc.ShogiCore.Snapshots
{
  /// <summary>Represents all possible shogi move types</summary>
  public enum MoveType
  {
    /// <summary>Move from one cell on board to another</summary>
    Usual,
    /// <summary>Drop from hand move</summary>
    Drop,
    /// <summary>Player resignation move</summary>
    Resign
  }

  /// <summary>Base class for lightweight snapshots of usual and drop moves</summary>
  [Serializable]
  public abstract class Move
  {
    /// <summary>Indicates if the move is usual or drop</summary>
    public abstract MoveType MoveType { get; }

    private RulesViolation _errorMessage;
    private BoardSnapshot _boardSnapshotAfter;

    /// <summary>Gets the color of player who made the move</summary>
    public abstract PieceColor Who { get; }
    
    /// <summary>Board snapshot before the move is done</summary>
    public BoardSnapshot BoardSnapshotBefore { get; private set; }

    /// <summary>Board snapshot after the move is done</summary>
    public BoardSnapshot BoardSnapshotAfter
    {
      get
      {
        Debug.Assert(_errorMessage == RulesViolation.NoViolations
                  || _errorMessage == RulesViolation.PartiallyValidated,
                  "Before you have Move.BoardSnapshotAfter Move should be at least partially validated");
        return _boardSnapshotAfter ?? (_boardSnapshotAfter = BoardSnapshotBefore.MakeMove(this));
      }
    }


    /// <summary>Indicates whether the move is valid</summary>
    public bool IsValid { get { return RulesViolation == RulesViolation.NoViolations; } }

    internal abstract RulesViolation Validate();

    protected Move(BoardSnapshot boardSnapshot)
    {
      if (boardSnapshot == null) throw new ArgumentNullException("boardSnapshot");

      BoardSnapshotBefore = boardSnapshot;
      
      var prevMove = boardSnapshot.Move;
      Number = prevMove == null ? 1 : prevMove.Number + 1;
    }

    /// <summary>null if move is valid -or- explanation why it's not</summary>
    public RulesViolation RulesViolation
    {
      get
      {
        if (_errorMessage == RulesViolation.HasntBeenChecked)
        {
          _errorMessage = RulesViolation.ValidationInProgress;
          _errorMessage = Validate(); 
        }
        return _errorMessage;
      }
    } 

    internal void MarkValid()
    {
      Debug.Assert(
           _errorMessage == RulesViolation.HasntBeenChecked 
        || _errorMessage == RulesViolation.PartiallyValidated
        || _errorMessage == RulesViolation.ValidationInProgress,
        "You're trying to mark invalid or validated move valid. There must be something wrong."
        );

      _errorMessage = RulesViolation.NoViolations;
    }

    internal void MarkPartiallyValid()
    {
      Debug.Assert(
           _errorMessage == RulesViolation.HasntBeenChecked
        || _errorMessage == RulesViolation.ValidationInProgress,
        "You're trying to mark invalid or valid move partially validated. There must be something wrong."
        );
      _errorMessage = RulesViolation.PartiallyValidated;
    }
    internal void CheckIsNotValidatedAtAll()
    {
      if (_errorMessage == RulesViolation.PartiallyValidated ||
          _errorMessage == RulesViolation.NoViolations)
        throw new Exception("Seems that we're validating something twice");
    }

    internal abstract void Apply(BoardSnapshot board);

    /// <summary>Move sequential number within the game</summary>
    public int Number { get; private set; }
  }
}