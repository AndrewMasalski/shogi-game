using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.Primitives;
using Yasc.Utils;

namespace Yasc.ShogiCore.Snapshots
{
  /// <summary>Represents lightweigth snapshot of the <see cref="Board"/></summary>
  [Serializable]
  public class BoardSnapshot
  {
    #region ' Fields '

    private bool _isHashCodeCalculated;
    private int _hashCode;
    private readonly IColoredPiece[,] _cells = new IColoredPiece[9, 9];
    private readonly List<IPieceType> _blackHand;
    private readonly List<IPieceType> _whiteHand;

    private ReadOnlySquareArray<IColoredPiece> _cellsRo;
    private ReadOnlyCollection<IPieceType> _blackHandRo;
    private ReadOnlyCollection<IPieceType> _whiteHandRo;

    private int HashCode
    {
      get
      {
        if (!_isHashCodeCalculated)
        {
          _hashCode = CalculateHashCode();
          _isHashCodeCalculated = true;
        }
        return _hashCode;
      }
    }

    #endregion

    #region ' Public Interface '

    /// <summary>The player who moves next</summary>
    public PieceColor OneWhoMoves { get; private set; }
    /// <summary>9x9 array of the cells with pieces</summary>
    public ReadOnlySquareArray<IColoredPiece> Cells
    {
      get { return _cellsRo ?? (_cellsRo = new ReadOnlySquareArray<IColoredPiece>(_cells)); }
    }
    /// <summary>List of the pieces in black hand</summary>
    public ReadOnlyCollection<IPieceType> BlackHand
    {
      get { return _blackHandRo ?? (_blackHandRo = new ReadOnlyCollection<IPieceType>(_blackHand)); }
    }
    /// <summary>List of the pieces in white hand</summary>
    public ReadOnlyCollection<IPieceType> WhiteHand
    {
      get { return _whiteHandRo ?? (_whiteHandRo = new ReadOnlyCollection<IPieceType>(_whiteHand)); }
    }

    /// <summary>ctor</summary>
    public BoardSnapshot(PieceColor oneWhoMoves,
      IEnumerable<Tuple<Position, IColoredPiece>> boardPieces,
      IEnumerable<IPieceType> whiteHand = null,
      IEnumerable<IPieceType> blackHand = null)
    {
      if (boardPieces == null) throw new ArgumentNullException("boardPieces");

      OneWhoMoves = oneWhoMoves;
      foreach (var t in boardPieces)
        SetPiece(t.Item1, t.Item2);

      _whiteHand = whiteHand != null ? whiteHand.ToList() : EmptyList<IPieceType>.Instance;
      _blackHand = blackHand != null ? blackHand.ToList() : EmptyList<IPieceType>.Instance; 
    }

    /// <summary>Creates a snapshot of the board with applied <paramref name="move"/></summary>
    public BoardSnapshot(BoardSnapshot board, Move move)
    {
      if (board == null) throw new ArgumentNullException("board");
      if (move == null) throw new ArgumentNullException("move");

      OneWhoMoves = board.OneWhoMoves;

      foreach (var p in Position.OnBoard)
        SetPiece(p, board.GetPieceAt(p));

      _whiteHand = board.WhiteHand.OrderBy(p => p).ToList();
      _blackHand = board.BlackHand.OrderBy(p => p).ToList();

      Move(move);
      OneWhoMoves = Opponent(OneWhoMoves);
    }

    /// <summary>Gets the piece snapshot at the <paramref name="position"/></summary>
    public IColoredPiece GetPieceAt(Position position)
    {
      return _cells[position.X, position.Y]; 
    }

    private void SetPiece(Position position, IColoredPiece value)
    {
      _cells[position.X, position.Y] = value;
    }
    /// <summary>Gets the piece snapshot at the coordinates</summary>
    public IColoredPiece GetPieceAt(int x, int y)
    {
      return _cells[x, y]; 
    }
    /// <summary>Gets the hand collection by color</summary>
    public ReadOnlyCollection<IPieceType> Hand(PieceColor color)
    {
      return color == PieceColor.White ? WhiteHand : BlackHand;
    }

    /// <summary>Validates drop move</summary>
    /// <returns>null if the move is valid -or- the reason why it's not</returns>
    public RulesViolation ValidateDropMove(DropMove move)
    {
      if (move == null) throw new ArgumentNullException("move");
      if (move.Piece.Color != OneWhoMoves) return RulesViolation.WrongSideToMove;
      if (!Hand(OneWhoMoves).Contains(move.Piece.PieceType)) return RulesViolation.WrongPieceReference;
      if (GetPieceAt(move.To) != null) return RulesViolation.DropToOccupiedCell;

      if (move.Piece.PieceType == PT.歩 || move.Piece.PieceType == PT.香)
        if (HowFarFromTheLastLine(move.Piece, move.To) == 0)
          return RulesViolation. DropToLastLines;

      if (move.Piece.PieceType == PT.桂)
        if (HowFarFromTheLastLine(move.Piece, move.To) < 2)
          return RulesViolation.DropToLastLines;

      if (move.Piece.PieceType == PT.歩)
        if (IsTherePawnOnThisColumn(OneWhoMoves, move.To.X))
          return RulesViolation.TwoPawnsOnTheSameFile;

      var newPosition = new BoardSnapshot(this, move);
      if (move.Piece.PieceType == PT.歩)
        if (newPosition.IsMateFor(Opponent(OneWhoMoves)))
          return RulesViolation.DropPawnToMate;

      return newPosition.IsCheckFor(OneWhoMoves) 
        ? RulesViolation.MoveToCheck 
        : RulesViolation.NoViolations;
    }
    /// <summary>Validates usual move</summary>
    /// <returns>null if the move is valid -or- the reason why it's not</returns>
    public RulesViolation ValidateUsualMove(UsualMove move)
    {
      if (move == null) throw new ArgumentNullException("move");
      var movingPiece = GetPieceAt(move.From);

      if (movingPiece == null)
        return RulesViolation.WrongPieceReference;

      if (move.From == move.To)
        return RulesViolation.PieceDoesntMoveThisWay;

      if (movingPiece.Color != OneWhoMoves)
        return RulesViolation.WrongSideToMove;

      var takenPiece = GetPieceAt(move.To);
      if (takenPiece != null)
        if (movingPiece.Color == takenPiece.Color)
          return RulesViolation.TakeAllyPiece;

      var analyser = EstimateUsualMoveTargets(move.From);
      if (!analyser.Contains(move.To))
        return RulesViolation.PieceDoesntMoveThisWay;

      return ShortValidateUsualMove(move);
    }

    /// <summary>Checks wheter it's mate for <paramref name="color"/> king </summary>
    public bool IsMateFor(PieceColor color)
    {
      return IsCheckFor(color) && DoesntHaveValidMoves(color);
    }
    /// <summary>Checks wheter it's check for <paramref name="color"/> king </summary>
    public bool IsCheckFor(PieceColor color)
    {
      var king = FindTheKing(color);
      return king != null &&
        GetAllValidUsualMovesWithoutCheck3(Opponent(color)).
        Any(move => move.To == king);
    }
    /// <summary>Gets all valid usual moves from the position</summary>
    public IEnumerable<UsualMove> GetAvailableUsualMoves(Position fromPosition)
    {
      var estimate = from p in EstimateUsualMoveTargets(fromPosition)
                     select new UsualMove(GetPieceAt(fromPosition).Color, fromPosition, p, false);
      return from move in DuplicateForPromoting(estimate)
             where ShortValidateUsualMove(move) == RulesViolation.NoViolations
             select move;
    }
    /// <summary>Gets all valid drop moves for the piece</summary>
    public IEnumerable<DropMove> GetAvailableDropMoves(IPieceType piece, PieceColor color)
    {
      return Position.OnBoard.Where(p => GetPieceAt(p) == null).
        Select(p => new DropMove(piece.GetColored(color), p)).
        Where(move => ValidateDropMove(move) == RulesViolation.NoViolations);
    }
    /// <summary>Gets all valid usual and drop moves for the player</summary>
    public IEnumerable<Move> GetAllAvailableMoves(PieceColor color)
    {
      foreach (var move in GetAllAvailableUsualMoves(color))
        yield return move;
      foreach (var move in GetAllValidDropMoves(color))
        yield return move;
    }

    #endregion

    #region ' Implemetation '

    private void Move(Move move)
    {
      var usual = move as UsualMove;
      if (usual != null)
      {
        Move(usual);
      }
      else
      {
        Move((DropMove)move);
      }
    }
    private void Move(DropMove move)
    {
      HandInternal(OneWhoMoves).Remove(move.Piece.PieceType);
      SetPiece(move.To, move.Piece);
    }
    private void Move(UsualMove move)
    {
      if (move.IsPromoting)
        SetPiece(move.From, GetPieceAt(move.From).Promoted);

      if (GetPieceAt(move.To) != null)
        HandInternal(OneWhoMoves).Add(GetPieceAt(move.To).PieceType);
      SetPiece(move.To, GetPieceAt(move.From));
      SetPiece(move.From, null);
    }
    private List<IPieceType> HandInternal(PieceColor color)
    {
      return color == PieceColor.White ? _whiteHand : _blackHand;
    }

    private IEnumerable<UsualMove> GetAllAvailableUsualMoves(PieceColor color)
    {
      return Position.OnBoard.Where(p => GetPieceAt(p) != null && GetPieceAt(p).Color == color).
        SelectMany(p => DuplicateForPromoting(GetAvailableUsualMoves(p)));
    }
    private IEnumerable<UsualMove> GetAllValidUsualMovesWithoutCheck3(PieceColor color)
    {
      return Position.OnBoard.Where(p => GetPieceAt(p) != null && GetPieceAt(p).Color == color).
        SelectMany(GetAllValidUsualMovesWithoutCheck3);
    }
    private IEnumerable<UsualMove> GetAllValidUsualMovesWithoutCheck3(Position f)
    {
      return from p in EstimateUsualMoveTargets(f)
             select new UsualMove(GetPieceAt(f).Color, f, p, false);
    }
    private IEnumerable<DropMove> GetAllValidDropMoves(PieceColor color)
    {
      return Hand(color).Distinct().SelectMany(p => GetAvailableDropMoves(p, color));
    }

    private bool DoesntHaveValidMoves(PieceColor color)
    {
      return GetAllAvailableMoves(color).FirstOrDefault() == null;
    }
    private Position? FindTheKing(PieceColor color)
    {
      return (from p in Position.OnBoard
              where GetPieceAt(p) != null
              where GetPieceAt(p).Color == color
              where GetPieceAt(p).PieceType.PieceKind == PT.K
              select (Position?)p).FirstOrDefault();
    }

    private IEnumerable<UsualMove> DuplicateForPromoting(IEnumerable<UsualMove> moves)
    {
      foreach (var m in moves)
      {
        if (IsPromotionMandatory(GetPieceAt(m.From), m.To) == RulesViolation.NoViolations)
          yield return new UsualMove(GetPieceAt(m.From).Color, m.From, m.To, false);

        if (IsPromotionAllowed(GetPieceAt(m.From), m.From, m.To) == RulesViolation.NoViolations)
          yield return new UsualMove(GetPieceAt(m.From).Color, m.From, m.To, true);
      }
    }
    private static PieceColor Opponent(PieceColor color)
    {
      return color == PieceColor.White ? PieceColor.Black : PieceColor.White;
    }
    private bool IsTherePawnOnThisColumn(PieceColor color, int column)
    {
      for (var i = 0; i < 9; i++)
        if (GetPieceAt(column, i) != null)
          if (GetPieceAt(column, i).PieceType == PT.歩)
            if (GetPieceAt(column, i).Color == color)
              return true;

      return false;
    }
    private RulesViolation ShortValidateUsualMove(UsualMove move)
    {
      if (move.IsPromoting)
      {
        var error = IsPromotionAllowed(GetPieceAt(move.From), move.From, move.To);
        if (error != RulesViolation.NoViolations)
          return error;
      }
      else
      {
        var error = IsPromotionMandatory(GetPieceAt(move.From), move.To);
        if (error != RulesViolation.NoViolations)
          return error;
      }

      var snapshot = new BoardSnapshot(this, move);
      return !snapshot.IsCheckFor(move.Who) 
        ? RulesViolation.NoViolations 
        : RulesViolation.MoveToCheck;
    }

    private int CalculateHashCode()
    {
      return EnumerableExtensions.GetSeqHashCode(
        OneWhoMoves.GetHashCode(),
        _cells.GetSeqHashCode(),
        _whiteHand.CalcHashCode(),
        _blackHand.CalcHashCode());
    }

    #endregion

    #region ' Equals '

    /// <summary>Indicates whether this instance and a specified object are equal.</summary>
    public bool Equals(BoardSnapshot other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return other.HashCode == HashCode &&
             Equals(other.OneWhoMoves, OneWhoMoves) &&
             EnumerableExtensions.Equal(other.Cells, Cells) &&
             EnumerableExtensions.Equal(other.BlackHand, BlackHand) &&
             EnumerableExtensions.Equal(other.WhiteHand, WhiteHand);
    }
    /// <summary>Indicates whether the piece type can be promoted</summary>
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      return obj.GetType() == typeof(BoardSnapshot) && Equals((BoardSnapshot)obj);
    }
    /// <summary>Returns the hash code for this instance.</summary>
    public override int GetHashCode()
    {
      return HashCode;
    }
    /// <summary>Indicates whether two instances are equal.</summary>
    public static bool operator ==(BoardSnapshot left, BoardSnapshot right)
    {
      return Equals(left, right);
    }
    /// <summary>Indicates whether two instances are not equal.</summary>
    public static bool operator !=(BoardSnapshot left, BoardSnapshot right)
    {
      return !Equals(left, right);
    }

    #endregion

    #region ' Estimate UsualMove Targets '

    private IEnumerable<Position> EstimateUsualMoveTargets(Position from)
    {
      return GetPieceAt(from).PieceType.MoveDirections.
        SelectMany(dir => Go(from, dir.Dx, dir.Dy, dir.Count));
    }
    private IEnumerable<Position> Go(Position from, int dx, int dy, int count)
    {
      var color = GetPieceAt(from).Color;
      var upDirection = color == PieceColor.White ?
        new Vector(1, 1) : new Vector(1, -1);
      var delta = new Vector(dx, dy) * upDirection;

      for (var curr = from + delta; count > 0; count--, curr += delta)
      {
        if (!curr.IsValidPosition)
        {
          yield break;
        }
        if (GetPieceAt(curr) == null)
        {
          yield return curr;
        }
        else if (GetPieceAt(curr).Color != color)
        {
          yield return curr;
          yield break;
        }
        else yield break;
      }
    }

    #endregion

    #region ' Initial Position '

    /// <summary>Contains initial position</summary>
    public static readonly BoardSnapshot InitialPosition = 
      new BoardSnapshot(PieceColor.Black, new[]{
        Tuple.Create(Position.Parse("1a"), PT.香.White),
        Tuple.Create(Position.Parse("9a"), PT.香.White),
        Tuple.Create(Position.Parse("1i"), PT.香.Black),
        Tuple.Create(Position.Parse("9i"), PT.香.Black),
        Tuple.Create(Position.Parse("2a"), PT.桂.White),
        Tuple.Create(Position.Parse("8a"), PT.桂.White),
        Tuple.Create(Position.Parse("2i"), PT.桂.Black),
        Tuple.Create(Position.Parse("8i"), PT.桂.Black),
        Tuple.Create(Position.Parse("3a"), PT.銀.White),
        Tuple.Create(Position.Parse("7a"), PT.銀.White),
        Tuple.Create(Position.Parse("3i"), PT.銀.Black),
        Tuple.Create(Position.Parse("7i"), PT.銀.Black),
        Tuple.Create(Position.Parse("4a"), PT.金.White),
        Tuple.Create(Position.Parse("6a"), PT.金.White),
        Tuple.Create(Position.Parse("4i"), PT.金.Black),
        Tuple.Create(Position.Parse("6i"), PT.金.Black),
        Tuple.Create(Position.Parse("5a"), PT.王.White),
        Tuple.Create(Position.Parse("5i"), PT.玉.Black),
        Tuple.Create(Position.Parse("2b"), PT.角.White),
        Tuple.Create(Position.Parse("8h"), PT.角.Black),
        Tuple.Create(Position.Parse("8b"), PT.飛.White),
        Tuple.Create(Position.Parse("2h"), PT.飛.Black),
        Tuple.Create(Position.Parse("1c"), PT.歩.White),
        Tuple.Create(Position.Parse("2c"), PT.歩.White),
        Tuple.Create(Position.Parse("3c"), PT.歩.White),
        Tuple.Create(Position.Parse("4c"), PT.歩.White),
        Tuple.Create(Position.Parse("5c"), PT.歩.White),
        Tuple.Create(Position.Parse("6c"), PT.歩.White),
        Tuple.Create(Position.Parse("7c"), PT.歩.White),
        Tuple.Create(Position.Parse("8c"), PT.歩.White),
        Tuple.Create(Position.Parse("9c"), PT.歩.White),
        Tuple.Create(Position.Parse("1g"), PT.歩.Black),
        Tuple.Create(Position.Parse("2g"), PT.歩.Black),
        Tuple.Create(Position.Parse("3g"), PT.歩.Black),
        Tuple.Create(Position.Parse("4g"), PT.歩.Black),
        Tuple.Create(Position.Parse("5g"), PT.歩.Black),
        Tuple.Create(Position.Parse("6g"), PT.歩.Black),
        Tuple.Create(Position.Parse("7g"), PT.歩.Black),
        Tuple.Create(Position.Parse("8g"), PT.歩.Black),
        Tuple.Create(Position.Parse("9g"), PT.歩.Black),
      });

    #endregion

    #region 

    /// <summary>Returns distance for from the last line for the piece (of the color)</summary>
    public int HowFarFromTheLastLine(IColoredPiece coloredPiece, Position position)
    {
      var lastLineIndex = coloredPiece.Color == PieceColor.White ? 8 : 0;
      return Math.Abs(position.Y - lastLineIndex);
    }
    /// <summary>Returns null it the piece can move to the 
    ///   <paramref name="position"/> without promotion -or- 
    ///   text with explanation why he's not allowed to do that</summary>
    public RulesViolation IsPromotionMandatory(IColoredPiece coloredPiece, Position position)
    {
      if (coloredPiece.PieceType == PT.歩 || coloredPiece.PieceType == PT.香)
        if (HowFarFromTheLastLine(coloredPiece, position) == 0)
          return RulesViolation.CantMoveWithoutPromotion;

      if (coloredPiece.PieceType == PT.桂)
        if (HowFarFromTheLastLine(coloredPiece, position) < 2)
          return RulesViolation.CantMoveWithoutPromotion;

      return RulesViolation.NoViolations;
    }
    /// <summary>Returns null it the piece can promote moving
    ///   from position <paramref name="from"/> to position <paramref name="to"/> -or- 
    ///   text with explanation why it's impossible</summary>
    public RulesViolation IsPromotionAllowed(IColoredPiece coloredPiece, Position from, Position to)
    {
      if (coloredPiece.PieceType.IsPromoted)
        return RulesViolation.CantPromoteTwice;

      if (!coloredPiece.PieceType.CanPromote)
        return RulesViolation.CantPromotePiecesOfThisType;

      if (!IsThatPromitionZoneFor(coloredPiece, from))
        if (!IsThatPromitionZoneFor(coloredPiece, to))
          return RulesViolation.CantPromoteWithThisMove;
      
      return RulesViolation.NoViolations;
    }

    private bool IsThatPromitionZoneFor(IColoredPiece coloredPiece, Position position)
    {
      return HowFarFromTheLastLine(coloredPiece, position) < 3;
    }

    #endregion
  }
}