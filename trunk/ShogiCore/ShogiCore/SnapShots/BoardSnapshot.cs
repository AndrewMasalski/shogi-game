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
    private readonly PieceSnapshot[,] _cells = new PieceSnapshot[9, 9];
    private readonly List<PieceSnapshot> _blackHand;
    private readonly List<PieceSnapshot> _whiteHand;

    private ReadOnlySquareArray<PieceSnapshot> _cellsRo;
    private ReadOnlyCollection<PieceSnapshot> _blackHandRo;
    private ReadOnlyCollection<PieceSnapshot> _whiteHandRo;

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
    public ReadOnlySquareArray<PieceSnapshot> Cells
    {
      get { return _cellsRo ?? (_cellsRo = new ReadOnlySquareArray<PieceSnapshot>(_cells)); }
    }
    /// <summary>List of the pieces in black hand</summary>
    public ReadOnlyCollection<PieceSnapshot> BlackHand
    {
      get { return _blackHandRo ?? (_blackHandRo = new ReadOnlyCollection<PieceSnapshot>(_blackHand)); }
    }
    /// <summary>List of the pieces in white hand</summary>
    public ReadOnlyCollection<PieceSnapshot> WhiteHand
    {
      get { return _whiteHandRo ?? (_whiteHandRo = new ReadOnlyCollection<PieceSnapshot>(_whiteHand)); }
    }

    /// <summary>ctor</summary>
    public BoardSnapshot(PieceColor oneWhoMoves,
      IEnumerable<Tuple<Position, PieceSnapshot>> boardPieces,
      IEnumerable<PieceSnapshot> whiteHand = null,
      IEnumerable<PieceSnapshot> blackHand = null)
    {
      if (boardPieces == null) throw new ArgumentNullException("boardPieces");

      OneWhoMoves = oneWhoMoves;
      foreach (var t in boardPieces)
        SetPiece(t.Item1, t.Item2);

      _whiteHand = whiteHand != null ? whiteHand.ToList() : EmptyList<PieceSnapshot>.Instance;
      _blackHand = blackHand != null ? blackHand.ToList() : EmptyList<PieceSnapshot>.Instance; 
    }

    /// <summary>Creates a snapshot of the board with applied <paramref name="move"/></summary>
    public BoardSnapshot(BoardSnapshot board, MoveSnapshotBase move)
    {
      if (board == null) throw new ArgumentNullException("board");
      if (move == null) throw new ArgumentNullException("move");

      OneWhoMoves = board.OneWhoMoves;

      foreach (var p in Position.OnBoard)
        SetPiece(p, board.GetPieceAt(p));

      _whiteHand = board.WhiteHand.OrderBy(p => p.PieceType).ToList();
      _blackHand = board.BlackHand.OrderBy(p => p.PieceType).ToList();

      Move(move);
      OneWhoMoves = Opponent(OneWhoMoves);
    }

    /// <summary>Gets the piece snapshot at the <paramref name="position"/></summary>
    public PieceSnapshot GetPieceAt(Position position)
    {
      return _cells[position.X, position.Y]; 
    }

    private void SetPiece(Position position, PieceSnapshot value)
    {
      _cells[position.X, position.Y] = value;
    }
    /// <summary>Gets the piece snapshot at the coordinates</summary>
    public PieceSnapshot GetPieceAt(int x, int y)
    {
      return _cells[x, y]; 
    }
    /// <summary>Gets the hand collection by color</summary>
    public ReadOnlyCollection<PieceSnapshot> Hand(PieceColor color)
    {
      return color == PieceColor.White ? WhiteHand : BlackHand;
    }

    /// <summary>Validates drop move</summary>
    /// <returns>null if the move is valid -or- the reason why it's not</returns>
    public string ValidateDropMove(DropMoveSnapshot move)
    {
      if (move == null) throw new ArgumentNullException("move");
      if (move.Piece.Color != OneWhoMoves) return "It's " + OneWhoMoves + "'s move now";
      if (!Hand(OneWhoMoves).Contains(move.Piece)) return "Player doesn't have this piece in hand";
      if (GetPieceAt(move.To) != null) return "Can drop piece to free cell only";

      if (move.Piece.PieceType == PieceType.歩 || move.Piece.PieceType == PieceType.香)
      {
        if (move.Piece.HowFarFromTheLastLine(move.To) == 0)
        {
          return "Can't drop " + move.Piece.PieceType + " to the last line";
        }
      }

      if (move.Piece.PieceType == PieceType.桂)
      {
        if (move.Piece.HowFarFromTheLastLine(move.To) < 2)
        {
          return "Can't drop 桂 to the last two lines";
        }
      }

      if (move.Piece.PieceType == PieceType.歩)
      {
        if (IsTherePawnOnThisColumn(OneWhoMoves, move.To.X))
        {
          return "Can't drop 歩 to the column " + move.To.Column + " because it already has one 歩";
        }
      }

      var newPosition = new BoardSnapshot(this, move);
      if (move.Piece.PieceType == PieceType.歩 && newPosition.IsMateFor(Opponent(OneWhoMoves)))
      {
        return "Can't drop 歩 to mate the opponent";
      }
      return !newPosition.IsCheckFor(OneWhoMoves) ? null :
        "If you made this move your king would be taken on the next move";
    }
    /// <summary>Validates usual move</summary>
    /// <returns>null if the move is valid -or- the reason why it's not</returns>
    public string ValidateUsualMove(UsualMoveSnapshot move)
    {
      if (move == null) throw new ArgumentNullException("move");
      var piece = GetPieceAt(move.From);

      if (piece == null)
        return "No piece at " + move.From;

      if (move.From == move.To)
        return "You can't move from " + move.From + " to " + move.To;

      if (piece.Color != OneWhoMoves)
        return "It's " + OneWhoMoves + "'s move now";

      if (GetPieceAt(move.To) != null)
        if (piece.Color == GetPieceAt(move.To).Color)
          return "Cant take piece of the same color";

      var analyser = EstimateUsualMoveTargets(move.From);
      if (!analyser.Contains(move.To))
      {
        return GetPieceAt(move.From).PieceType + " doesn't move this way";
      }

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
    public IEnumerable<UsualMoveSnapshot> GetAvailableUsualMoves(Position fromPosition)
    {
      var estimate = from p in EstimateUsualMoveTargets(fromPosition)
                     select new UsualMoveSnapshot(GetPieceAt(fromPosition).Color, fromPosition, p, false);
      return from move in DuplicateForPromoting(estimate)
             where ShortValidateUsualMove(move) == null
             select move;
    }
    /// <summary>Gets all valid drop moves for the piece</summary>
    public IEnumerable<DropMoveSnapshot> GetAvailableDropMoves(PieceSnapshot piece)
    {
      return Position.OnBoard.Where(p => GetPieceAt(p) == null).
        Select(p => new DropMoveSnapshot(piece, p)).
        Where(move => ValidateDropMove(move) == null);
    }
    /// <summary>Gets all valid usual and drop moves for the player</summary>
    public IEnumerable<MoveSnapshotBase> GetAllAvailableMoves(PieceColor color)
    {
      foreach (var move in GetAllAvailableUsualMoves(color))
        yield return move;
      foreach (var move in GetAllValidDropMoves(color))
        yield return move;
    }

    #endregion

    #region ' Implemetation '

    private void Move(MoveSnapshotBase move)
    {
      var usual = move as UsualMoveSnapshot;
      if (usual != null)
      {
        Move(usual);
      }
      else
      {
        Move((DropMoveSnapshot)move);
      }
    }
    private void Move(DropMoveSnapshot move)
    {
      HandInternal(OneWhoMoves).Remove(move.Piece);
      SetPiece(move.To, move.Piece);
    }
    private void Move(UsualMoveSnapshot move)
    {
      if (move.IsPromoting)
        SetPiece(move.From, GetPieceAt(move.From).ClonePromoted());

      if (GetPieceAt(move.To) != null)
        HandInternal(OneWhoMoves).Add(GetPieceAt(move.To));
      SetPiece(move.To, GetPieceAt(move.From));
      SetPiece(move.From, null);
    }
    private List<PieceSnapshot> HandInternal(PieceColor color)
    {
      return color == PieceColor.White ? _whiteHand : _blackHand;
    }

    private IEnumerable<UsualMoveSnapshot> GetAllAvailableUsualMoves(PieceColor color)
    {
      return Position.OnBoard.Where(p => GetPieceAt(p) != null && GetPieceAt(p).Color == color).
        SelectMany(p => DuplicateForPromoting(GetAvailableUsualMoves(p)));
    }
    private IEnumerable<UsualMoveSnapshot> GetAllValidUsualMovesWithoutCheck3(PieceColor color)
    {
      return Position.OnBoard.Where(p => GetPieceAt(p) != null && GetPieceAt(p).Color == color).
        SelectMany(GetAllValidUsualMovesWithoutCheck3);
    }
    private IEnumerable<UsualMoveSnapshot> GetAllValidUsualMovesWithoutCheck3(Position f)
    {
      return from p in EstimateUsualMoveTargets(f)
             select new UsualMoveSnapshot(GetPieceAt(f).Color, f, p, false);
    }
    private IEnumerable<DropMoveSnapshot> GetAllValidDropMoves(PieceColor color)
    {
      return Hand(color).Distinct().SelectMany(GetAvailableDropMoves);
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
              //TODO: Does anyone checks that there are two options here?
              where GetPieceAt(p).PieceType == PieceType.玉 || GetPieceAt(p).PieceType == PieceType.王
              select (Position?)p).FirstOrDefault();
    }

    private IEnumerable<UsualMoveSnapshot> DuplicateForPromoting(IEnumerable<UsualMoveSnapshot> moves)
    {
      foreach (var m in moves)
      {
        if (GetPieceAt(m.From).IsPromotionMandatory(m.To) == null)
          yield return new UsualMoveSnapshot(GetPieceAt(m.From).Color, m.From, m.To, false);

        if (GetPieceAt(m.From).IsPromotionAllowed(m.From, m.To) == null)
          yield return new UsualMoveSnapshot(GetPieceAt(m.From).Color, m.From, m.To, true);
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
          if (GetPieceAt(column, i).PieceType == PieceType.歩)
            if (GetPieceAt(column, i).Color == color)
              return true;

      return false;
    }
    private string ShortValidateUsualMove(UsualMoveSnapshot move)
    {
      if (move.IsPromoting)
      {
        var error = GetPieceAt(move.From).IsPromotionAllowed(move.From, move.To);
        if (error != null)
          return error;
      }
      else
      {
        var error = GetPieceAt(move.From).IsPromotionMandatory(move.To);
        if (error != null)
          return error;
      }

      var snapshot = new BoardSnapshot(this, move);
      return !snapshot.IsCheckFor(move.Who) ? null :
        "If you made this move your king would be taken on the next move";
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

    private const int MaxMoveLength = 8;

    private IEnumerable<Position> EstimateUsualMoveTargets(Position from)
    {
      switch (GetPieceAt(from).PieceType.ToString())
      {
        case "王": return GetMovesFor王(from);
        case "玉": return GetMovesFor王(from);
        case "飛": return GetMovesFor飛(from);
        case "角": return GetMovesFor角(from);
        case "金": return GetMovesFor金(from);
        case "銀": return GetMovesFor銀(from);
        case "桂": return GetMovesFor桂(from);
        case "香": return GetMovesFor香(from);
        case "歩": return GetMovesFor歩(from);
        case "竜": return GetMovesFor竜(from);
        case "馬": return GetMovesFor馬(from);
        case "全": return GetMovesFor金(from);
        case "今": return GetMovesFor金(from);
        case "仝": return GetMovesFor金(from);
        default:
          /*case "と":*/
          return GetMovesFor金(from);
      }
    }

    private IEnumerable<Position> GetMovesFor馬(Position from)
    {
      return Join(GetMovesFor角(from),
                  Up(from, 1), Right(from, 1), Down(from, 1), Left(from, 1));
    }
    private IEnumerable<Position> GetMovesFor竜(Position from)
    {
      return Join(GetMovesFor飛(from),
                  UpLeft(from, 1), UpRight(from, 1), DownRight(from, 1), DownLeft(from, 1));
    }
    private IEnumerable<Position> GetMovesFor歩(Position from)
    {
      return Up(from, 1);
    }
    private IEnumerable<Position> GetMovesFor香(Position from)
    {
      return Up(from, MaxMoveLength);
    }
    private IEnumerable<Position> GetMovesFor桂(Position from)
    {
      return Join(Go(from, 1, 2, 1), Go(from, -1, 2, 1));
    }
    private IEnumerable<Position> GetMovesFor銀(Position from)
    {
      return Join(Up(from, 1), UpRight(from, 1),
                  DownRight(from, 1), DownLeft(from, 1), UpLeft(from, 1));
    }
    private IEnumerable<Position> GetMovesFor金(Position from)
    {
      return Join(Up(from, 1), UpRight(from, 1),
                  Right(from, 1), Down(from, 1), Left(from, 1), UpLeft(from, 1));
    }
    private IEnumerable<Position> GetMovesFor角(Position from)
    {
      return Join(UpRight(from, MaxMoveLength),
                  DownRight(from, MaxMoveLength), DownLeft(from, MaxMoveLength),
                  UpLeft(from, MaxMoveLength));
    }
    private IEnumerable<Position> GetMovesFor飛(Position from)
    {
      return Join(Up(from, MaxMoveLength),
                  Right(from, MaxMoveLength), Down(from, MaxMoveLength),
                  Left(from, MaxMoveLength));
    }
    private IEnumerable<Position> GetMovesFor王(Position from)
    {
      return Join(Up(from, 1), UpRight(from, 1), Right(from, 1),
                  DownRight(from, 1), Down(from, 1), DownLeft(from, 1),
                  Left(from, 1), UpLeft(from, 1));
    }

    private IEnumerable<Position> Up(Position from, int i)
    {
      return Go(from, 0, 1, i);
    }
    private IEnumerable<Position> UpRight(Position from, int i)
    {
      return Go(from, 1, 1, i);
    }
    private IEnumerable<Position> Right(Position from, int i)
    {
      return Go(from, 1, 0, i);
    }
    private IEnumerable<Position> DownRight(Position from, int i)
    {
      return Go(from, 1, -1, i);
    }
    private IEnumerable<Position> Down(Position from, int i)
    {
      return Go(from, 0, -1, i);
    }
    private IEnumerable<Position> DownLeft(Position from, int i)
    {
      return Go(from, -1, -1, i);
    }
    private IEnumerable<Position> Left(Position from, int i)
    {
      return Go(from, -1, 0, i);
    }
    private IEnumerable<Position> UpLeft(Position from, int i)
    {
      return Go(from, -1, 1, i);
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

    private static IEnumerable<T> Join<T>(params IEnumerable<T>[] arr)
    {
      return arr.SelectMany(e => e);
    }

    #endregion

    #region ' Initial Position '

    /// <summary>Contains initial position</summary>
    public static readonly BoardSnapshot InitialPosition = 
      new BoardSnapshot(PieceColor.Black, new[]{
        Tuple.Create(Position.Parse("1a"), new PieceSnapshot(PieceType.香, PieceColor.White)),
        Tuple.Create(Position.Parse("9a"), new PieceSnapshot(PieceType.香, PieceColor.White)),
        Tuple.Create(Position.Parse("1i"), new PieceSnapshot(PieceType.香, PieceColor.Black)),
        Tuple.Create(Position.Parse("9i"), new PieceSnapshot(PieceType.香, PieceColor.Black)),
        Tuple.Create(Position.Parse("2a"), new PieceSnapshot(PieceType.桂, PieceColor.White)),
        Tuple.Create(Position.Parse("8a"), new PieceSnapshot(PieceType.桂, PieceColor.White)),
        Tuple.Create(Position.Parse("2i"), new PieceSnapshot(PieceType.桂, PieceColor.Black)),
        Tuple.Create(Position.Parse("8i"), new PieceSnapshot(PieceType.桂, PieceColor.Black)),
        Tuple.Create(Position.Parse("3a"), new PieceSnapshot(PieceType.銀, PieceColor.White)),
        Tuple.Create(Position.Parse("7a"), new PieceSnapshot(PieceType.銀, PieceColor.White)),
        Tuple.Create(Position.Parse("3i"), new PieceSnapshot(PieceType.銀, PieceColor.Black)),
        Tuple.Create(Position.Parse("7i"), new PieceSnapshot(PieceType.銀, PieceColor.Black)),
        Tuple.Create(Position.Parse("4a"), new PieceSnapshot(PieceType.金, PieceColor.White)),
        Tuple.Create(Position.Parse("6a"), new PieceSnapshot(PieceType.金, PieceColor.White)),
        Tuple.Create(Position.Parse("4i"), new PieceSnapshot(PieceType.金, PieceColor.Black)),
        Tuple.Create(Position.Parse("6i"), new PieceSnapshot(PieceType.金, PieceColor.Black)),
        Tuple.Create(Position.Parse("5a"), new PieceSnapshot(PieceType.王, PieceColor.White)),
        Tuple.Create(Position.Parse("5i"), new PieceSnapshot(PieceType.玉, PieceColor.Black)),
        Tuple.Create(Position.Parse("2b"), new PieceSnapshot(PieceType.角, PieceColor.White)),
        Tuple.Create(Position.Parse("8h"), new PieceSnapshot(PieceType.角, PieceColor.Black)),
        Tuple.Create(Position.Parse("8b"), new PieceSnapshot(PieceType.飛, PieceColor.White)),
        Tuple.Create(Position.Parse("2h"), new PieceSnapshot(PieceType.飛, PieceColor.Black)),
        Tuple.Create(Position.Parse("1c"), new PieceSnapshot(PieceType.歩, PieceColor.White)),
        Tuple.Create(Position.Parse("2c"), new PieceSnapshot(PieceType.歩, PieceColor.White)),
        Tuple.Create(Position.Parse("3c"), new PieceSnapshot(PieceType.歩, PieceColor.White)),
        Tuple.Create(Position.Parse("4c"), new PieceSnapshot(PieceType.歩, PieceColor.White)),
        Tuple.Create(Position.Parse("5c"), new PieceSnapshot(PieceType.歩, PieceColor.White)),
        Tuple.Create(Position.Parse("6c"), new PieceSnapshot(PieceType.歩, PieceColor.White)),
        Tuple.Create(Position.Parse("7c"), new PieceSnapshot(PieceType.歩, PieceColor.White)),
        Tuple.Create(Position.Parse("8c"), new PieceSnapshot(PieceType.歩, PieceColor.White)),
        Tuple.Create(Position.Parse("9c"), new PieceSnapshot(PieceType.歩, PieceColor.White)),
        Tuple.Create(Position.Parse("1g"), new PieceSnapshot(PieceType.歩, PieceColor.Black)),
        Tuple.Create(Position.Parse("2g"), new PieceSnapshot(PieceType.歩, PieceColor.Black)),
        Tuple.Create(Position.Parse("3g"), new PieceSnapshot(PieceType.歩, PieceColor.Black)),
        Tuple.Create(Position.Parse("4g"), new PieceSnapshot(PieceType.歩, PieceColor.Black)),
        Tuple.Create(Position.Parse("5g"), new PieceSnapshot(PieceType.歩, PieceColor.Black)),
        Tuple.Create(Position.Parse("6g"), new PieceSnapshot(PieceType.歩, PieceColor.Black)),
        Tuple.Create(Position.Parse("7g"), new PieceSnapshot(PieceType.歩, PieceColor.Black)),
        Tuple.Create(Position.Parse("8g"), new PieceSnapshot(PieceType.歩, PieceColor.Black)),
        Tuple.Create(Position.Parse("9g"), new PieceSnapshot(PieceType.歩, PieceColor.Black)),
      });

    #endregion
  }
  /// <summary>Set of extension methods which makes work with 
  ///   grid handy althogh don't add any functionality</summary>
  public static class BoardSnapshotExtensions
  {
    /// <summary>Gets the piece snapshot at the <paramref name="position"/></summary>
    public static PieceSnapshot GetPieceAt(this BoardSnapshot board, string position)
    {
      return board.GetPieceAt(Position.Parse(position));
    }
  }
}