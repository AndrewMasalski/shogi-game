using System;
using System.Diagnostics;
using Yasc.ShogiCore.Primitives;

namespace Yasc.ShogiCore.Snapshots
{
  public enum MoveType
  {
    Usual, Drop, Resign
  }
  /// <summary>Base class for lightweight snapshots of usual and drop moves</summary>
  [Serializable]
  public abstract class Move
  {
    public abstract MoveType MoveType { get; }
    private RulesViolation _errorMessage;

    /// <summary>Gets the color of player who made the move</summary>
    public abstract PieceColor Who { get; }
    
    /// <summary>Board snapshot before the move is done</summary>
    public BoardSnapshot BoardSnapshot { get; private set; }

    /// <summary>Indicates whether the move is valid</summary>
    public bool IsValid { get { return RulesViolation == RulesViolation.NoViolations; } }

    internal abstract RulesViolation Validate();

    protected Move(BoardSnapshot boardSnapshot)
    {
      if (boardSnapshot == null) throw new ArgumentNullException("boardSnapshot");

      BoardSnapshot = boardSnapshot;
      
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
          _errorMessage = Validate(); 
        }
        return _errorMessage;
      }
    } 

    internal void MarkValid()
    {
      Debug.Assert(_errorMessage == RulesViolation.HasntBeenChecked);
      _errorMessage = RulesViolation.NoViolations;
    }

    internal abstract void Apply(BoardSnapshot board);

    /// <summary>Move sequential number within the game</summary>
    public int Number { get; private set; }
  }
}