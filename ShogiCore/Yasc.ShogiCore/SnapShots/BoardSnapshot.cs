using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
      get
      {
        if (_cellsRo == null)
        {
          _cellsRo = new ReadOnlySquareArray<PieceSnapshot>(_cells);
        }
        return _cellsRo;
      }
    }
    /// <summary>List of the pieces in black hand</summary>
    public ReadOnlyCollection<PieceSnapshot> BlackHand
    {
      get
      {
        if (_blackHandRo == null)
        {
          _blackHandRo = new ReadOnlyCollection<PieceSnapshot>(_blackHand);
        }
        return _blackHandRo;
      }
    }
    /// <summary>List of the pieces in white hand</summary>
    public ReadOnlyCollection<PieceSnapshot> WhiteHand
    {
      get
      {
        if (_whiteHandRo == null)
        {
          _whiteHandRo = new ReadOnlyCollection<PieceSnapshot>(_whiteHand);
        }
        return _whiteHandRo;
      }
    }

    /// <summary>ctor</summary>
    public BoardSnapshot(Board board)
    {
      OneWhoMoves = board.OneWhoMoves.Color;

      foreach (var p in Position.OnBoard)
        this[p] = board[p] == null ? null : new PieceSnapshot(board[p]);

      _whiteHand = (from p in board.White.Hand
                    orderby p.PieceType
                    select new PieceSnapshot(p)).ToList();
      _blackHand = (from p in board.Black.Hand
                    orderby p.PieceType
                    select new PieceSnapshot(p)).ToList();

    }
    /// <summary>Creates a snapshot of the board with applied <paramref name="move"/></summary>
    public BoardSnapshot(BoardSnapshot board, MoveSnapshotBase move)
    {
      OneWhoMoves = board.OneWhoMoves;

      foreach (var p in Position.OnBoard)
        this[p] = board[p];

      _whiteHand = board.WhiteHand.OrderBy(p => p.PieceType).ToList();
      _blackHand = board.BlackHand.OrderBy(p => p.PieceType).ToList();

      if (move != null)
      {
        if (move is UsualMoveSnapshot)
        {
          Move((UsualMoveSnapshot)move);
        }
        else
        {
          Move((DropMoveSnapshot)move);
        }
        OneWhoMoves = Opponent(OneWhoMoves);
      }
    }

    /// <summary>Gets the piece snapshot at the <paramref name="position"/></summary>
    public PieceSnapshot this[Position position]
    {
      get { return _cells[position.X, position.Y]; }
      private set { _cells[position.X, position.Y] = value; }
    }
    /// <summary>Gets the piece snapshot at the coordinates</summary>
    public PieceSnapshot this[int x, int y]
    {
      get { return _cells[x, y]; }
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
      if (move.Piece.Color != OneWhoMoves) return "It's " + OneWhoMoves + "'s move now";
      if (!Hand(OneWhoMoves).Contains(move.Piece)) return "Player doesn't have this piece in hand";
      if (this[move.To] != null) return "Can drop piece to free cell only";

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
      if (newPosition.IsCheckFor(OneWhoMoves))
      {
        return "If you made this move your king would be taken on the next move";
      }
      return null;
    }
    /// <summary>Validates usual move</summary>
    /// <returns>null if the move is valid -or- the reason why it's not</returns>
    public string ValidateUsualMove(UsualMoveSnapshot move)
    {
      var piece = this[move.From];

      if (piece == null)
        return "No piece at " + move.From;

      if (move.From == move.To)
        return "You can't move from " + move.From + " to " + move.To;

      if (piece.Color != OneWhoMoves)
        return "It's " + OneWhoMoves + "'s move now";

      if (this[move.To] != null)
        if (piece.Color == this[move.To].Color)
          return "Cant take piece of the same color";

      var analyser = EstimateUsualMoveTargets(move.From);
      if (!analyser.Contains(move.To))
      {
        return this[move.From].PieceType + " doesn't move this way";
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
      if (king == null) return false;
      foreach (var move in GetAllValidUsualMovesWithoutCheck3(Opponent(color)))
        if (move.To == king)
          return true;
      return false;
    }
    /// <summary>Gets all valid usual moves from the position</summary>
    public IEnumerable<UsualMoveSnapshot> GetAvailableUsualMoves(Position fromPosition)
    {
      var estimate = from p in EstimateUsualMoveTargets(fromPosition)
                     select new UsualMoveSnapshot(fromPosition, p, false);
      return from move in DuplicateForPromoting(estimate)
             where ShortValidateUsualMove(move) == null
             select move;
    }
    /// <summary>Gets all valid drop moves for the piece</summary>
    public IEnumerable<DropMoveSnapshot> GetAvailableDropMoves(PieceSnapshot piece)
    {
      foreach (var p in Position.OnBoard)
        if (this[p] == null)
        {
          var move = new DropMoveSnapshot(piece, p);
          if (ValidateDropMove(move) == null)
            yield return move;
        }
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

    private void Move(DropMoveSnapshot move)
    {
      HandInternal(OneWhoMoves).Remove(move.Piece);
      this[move.To] = move.Piece;
    }
    private void Move(UsualMoveSnapshot move)
    {
      if (move.IsPromoting)
        this[move.From] = this[move.From].ClonePromoted();

      if (this[move.To] != null)
        HandInternal(OneWhoMoves).Add(this[move.To]);
      this[move.To] = this[move.From];
      this[move.From] = null;
    }
    private List<PieceSnapshot> HandInternal(PieceColor color)
    {
      return color == PieceColor.White ? _whiteHand : _blackHand;
    }
    
    private IEnumerable<UsualMoveSnapshot> GetAllAvailableUsualMoves(PieceColor color)
    {
      foreach (var p in Position.OnBoard)
        if (this[p] != null && this[p].Color == color)
          foreach (var move in DuplicateForPromoting(GetAvailableUsualMoves(p)))
            yield return move;
    }
    private IEnumerable<UsualMoveSnapshot> GetAllValidUsualMovesWithoutCheck3(PieceColor color)
    {
      foreach (var p in Position.OnBoard)
        if (this[p] != null && this[p].Color == color)
          foreach (var move in GetAllValidUsualMovesWithoutCheck3(p))
            yield return move;
    }
    private IEnumerable<UsualMoveSnapshot> GetAllValidUsualMovesWithoutCheck3(Position f)
    {
      return from p in EstimateUsualMoveTargets(f)
             select new UsualMoveSnapshot(f, p, false);
    }
    private IEnumerable<DropMoveSnapshot> GetAllValidDropMoves(PieceColor color)
    {
      foreach (var piece in Hand(color).Distinct())
        foreach (var move in GetAvailableDropMoves(piece))
          yield return move;
    }

    private bool DoesntHaveValidMoves(PieceColor color)
    {
      return GetAllAvailableMoves(color).FirstOrDefault() == null;
    }
    private Position? FindTheKing(PieceColor color)
    {
      foreach (var p in Position.OnBoard)
        if (this[p] != null)
          if (this[p].Color == color)
//TODO: Does anyone checks that there's two options here?
            if (this[p].PieceType == PieceType.玉 || this[p].PieceType == PieceType.王)
              return p;

      return null;
    }
    private IEnumerable<UsualMoveSnapshot> DuplicateForPromoting(IEnumerable<UsualMoveSnapshot> moves)
    {
      foreach (var m in moves)
      {
        if (this[m.From].IsPromotionMandatory(m.To) == null)
          yield return new UsualMoveSnapshot(m.From, m.To, false);

        if (this[m.From].IsPromotionAllowed(m.From, m.To) == null)
          yield return new UsualMoveSnapshot(m.From, m.To, true);
      }
    }
    private static PieceColor Opponent(PieceColor color)
    {
      return color == PieceColor.White ? PieceColor.Black : PieceColor.White;
    }
    private bool IsTherePawnOnThisColumn(PieceColor color, int column)
    {
      for (int i = 0; i < 9; i++)
        if (this[column, i] != null)
          if (this[column, i].PieceType == PieceType.歩)
            if (this[column, i].Color == color)
              return true;

      return false;
    }
    private string ShortValidateUsualMove(UsualMoveSnapshot move)
    {
      if (move.IsPromoting)
      {
        var error = this[move.From].IsPromotionAllowed(move.From, move.To);
        if (error != null)
          return error;
      }
      else
      {
        var error = this[move.From].IsPromotionMandatory(move.To);
        if (error != null)
          return error;
      }

      var snapshot = new BoardSnapshot(this, move);
      return !snapshot.IsCheckFor(move.GetColor(this)) ? null :
        "If you made this move your king would be taken on the next move";
    }

    private int CalculateHashCode()
    {
      return ListUtils.GetSeqHashCode(
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
             ListUtils.Equal(other.Cells, Cells) &&
             ListUtils.Equal(other.BlackHand, BlackHand) &&
             ListUtils.Equal(other.WhiteHand, WhiteHand);
    }
    /// <summary>Indicates whether the piece type can be promoted</summary>
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != typeof(BoardSnapshot)) return false;
      return Equals((BoardSnapshot)obj);
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
      switch ((string)this[from].PieceType)
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
        case "と": return GetMovesFor金(from);
      }
      throw new Exception();
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
      var color = this[from].Color;
      var upDirection = color == PieceColor.White ?
                                                    new Vector(1, 1) : new Vector(1, -1);
      var delta = new Vector(dx, dy) * upDirection;

      for (var curr = from + delta; count > 0; count--, curr += delta)
      {
        if (!curr.IsValidPosition)
        {
          yield break;
        }
        if (this[curr] == null)
        {
          yield return curr;
        }
        else if (this[curr].Color != color)
        {
          yield return curr;
          yield break;
        }
        else yield break;
      }
    }

    private static IEnumerable<T> Join<T>(params IEnumerable<T>[] arr)
    {
      foreach (var enumerable in arr)
        foreach (var item in enumerable)
          yield return item;
    }

    #endregion
  }
}