namespace Yasc.ShogiCore.Snapshots
{
  /// <summary>Defines possible states move can be before or after it's validated</summary>
  public enum RulesViolation
  {
    /// <summary>Move hasn't been validated yet</summary>
    HasntBeenChecked,
    /// <summary>Move is valid</summary>
    NoViolations,
    /// <summary>Drop pawn to the file which already has one upromoted pawn</summary>
    TwoPawnsOnTheSameFile,
    /// <summary>Drop pawn to mate opponent's king</summary>
    DropPawnToMate,
    /// <summary>Move which leaves king under check</summary>
    MoveToCheck,
    /// <summary>Drop pawn or lance to the last lines</summary>
    DropToLastLines,
    /// <summary>Drop piece to the cell which is already occupied</summary>
    DropToOccupiedCell,
    /// <summary>Move from empty cell -or- drop of piece player hasn't got in hand</summary>
    /// <remarks>For a drop might indicate that you're moving for a wrong side</remarks>
    WrongPieceReference,
    /// <summary>It is not your turn to move</summary>
    WrongSideToMove,
    /// <summary>Usual move of piece to the cell it cannot move to</summary>
    /// <remarks>For example pawn cannot move backwards, rook cannot jump over pieces</remarks>
    PieceDoesntMoveThisWay,
    /// <summary>Usual move to the cell which is already occupied with piece of the same color</summary>
    TakeAllyPiece,
    /// <summary>Usual move of promoted piece with "promotion is requested" flag</summary>
    CantPromoteTwice,
    /// <summary>"Promotion is requested" flag with move of king or gold</summary>
    CantPromotePiecesOfThisType,
    /// <summary>"Promotion is requested" flag with move outside the opponent's camp</summary>
    CantPromoteWithThisMove,
    /// <summary>Move pawn or lance to the last lines without promotion</summary>
    CantMoveWithoutPromotion,
    /// <summary>Cannot make perpetual check</summary>
    PerpetualCheck,
    ValidationInProgress,
    PartiallyValidated,
  }
}