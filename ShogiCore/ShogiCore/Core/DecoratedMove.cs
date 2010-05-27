using System;
using Yasc.ShogiCore.Primitives;
using Yasc.ShogiCore.Snapshots;

namespace Yasc.ShogiCore.Core
{
  /// <summary>Base class for the shogi move (usual and drop)</summary>
  public class DecoratedMove
  {
    /// <summary>Moment move is performed</summary>
    public DateTime Timestamp { get; private set; }

    /// <summary>Gets the color of player who made the move</summary>
    public PieceColor Who { get { return Move.Who; } }
    /// <summary>Move sequential number within the game</summary>
    public int Number { get; private set; }
    /// <summary>Board snapshot before the move is done</summary>
    public BoardSnapshot BoardSnapshot { get; private set; }
    /// <summary>Gets the move data</summary>
    public Move Move { get; set; }

    /// <summary>Indicates whether the move is valid</summary>
    public bool IsValid { get { return RulesViolation == RulesViolation.NoViolations; } }

    private RulesViolation _errorMessage;

    /// <summary>null if move is valid -or- explanation why it's not</summary>
    public RulesViolation RulesViolation
    {
      get
      {
        if (_errorMessage == RulesViolation.HasntBeenChecked)
        {
          var dropMove = Move as DropMove;
          if (dropMove != null)
          {
            _errorMessage = BoardSnapshot.ValidateDropMove(dropMove);
          }
          var usualMove = Move as UsualMove;
          if (usualMove != null)
          {
            _errorMessage = BoardSnapshot.ValidateUsualMove(usualMove);
          }
          var resignMove = Move as ResignMove;
          if (resignMove != null)
          {
            _errorMessage = resignMove.Who != BoardSnapshot.OneWhoMoves
              ? RulesViolation.WrongSideToMove : RulesViolation.NoViolations;
          }
        }
        return _errorMessage;
      }
    }
    /// <summary>ctor</summary>
    internal DecoratedMove(BoardSnapshot boardSnapshot, Move move, int number)
    {
      if (boardSnapshot == null) throw new ArgumentNullException("boardSnapshot");
      
      Timestamp = DateTime.Now;
      Number = number;
      BoardSnapshot = boardSnapshot;
      Move = move;
    }

    public override string ToString()
    {
      return Move.ToString();
    }
  }
}