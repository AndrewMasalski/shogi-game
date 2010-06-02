using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
    private KnownPositions _knownPositions;
    
    private readonly IColoredPiece[,] _cells = new IColoredPiece[9, 9];
    private readonly List<IPieceType> _blackHand;
    private readonly List<IPieceType> _whiteHand;

    private ReadOnlySquareArray<IColoredPiece> _cellsRo;
    private ReadOnlyCollection<IPieceType> _blackHandRo;
    private ReadOnlyCollection<IPieceType> _whiteHandRo;

    private int _hashCode;
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
    public PieceColor SideOnMove { get; private set; }
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
    /// <summary>Gets reference to the move leaded to the current position 
    /// -or- null if current position is custom</summary>
    public Move Move { get; private set; }
    /// <summary>Gets the game result (or <see cref="ShogiGameState.None"/> if game is not finished)</summary>
    public ShogiGameState GameState { get; internal set; }

    /// <summary>Parses SFEN string</summary>
    /// <param name="sfenString">lnsgkgsn1/1r5b1/ppppppppp/9/9/9/PPPPPPPPP/1B5R1/LNSGKGSNL w - 1</param>
    /// <exception cref="InvalidOperationException">Can't promote pieceType</exception>
    public static BoardSnapshot ParseSfen(string sfenString)
    {
      if (sfenString == null) throw new ArgumentNullException("sfenString");
      var records = sfenString.Split(' ');
      if (records.Length < 3 || records.Length > 4)
        throw new ArgumentOutOfRangeException("sfenString", "Must consist of 3 or 4 space separated records");
      var board = records[0];
      var sideOnMove = records[1];
      var hand = records[2];
      var rows = board.Split('/');
      if (rows.Length != 9)
        throw new ArgumentOutOfRangeException("sfenString", "Board record must consist of 9 slash separated rows");

      var res = new BoardSnapshot(new List<IPieceType>(), new List<IPieceType>());
      for (int rowIndex = 0; rowIndex < 9; rowIndex++)
      {
        var currentFile = 0;
        var promoted = false;
        foreach (var ch in rows[rowIndex])
        {
          if (char.IsDigit(ch))
          {
            currentFile += int.Parse(ch.ToString());
            if (currentFile > 9)
              throw new ArgumentOutOfRangeException("sfenString", "Row contains more than 9 cells");
          }
          else if (ch == '+')
          {
            promoted = true;
          }
          else
          {
            IPieceType pieceType;
            switch (ch)
            {
              case 'k':
                pieceType = PT.王;
                break;
              case 'K':
                pieceType = PT.玉;
                break;
              default:
                if (!PT.TryParse(char.ToUpper(ch).ToString(), out pieceType))
                  throw new ArgumentOutOfRangeException("sfenString", "Piece type is not found: " + ch);
                break;
            }

            if (promoted)
              pieceType = pieceType.Promote();

            var piece = char.IsUpper(ch) ? pieceType.Black : pieceType.White;

            res._cells[8 - currentFile++, rowIndex] = piece;
          }
        }
        if (currentFile != 9)
          throw new ArgumentOutOfRangeException("sfenString", "Row must contain exactly 9 cells");
      }
      res.SideOnMove = sideOnMove.ToLower() == "w" ? PieceColor.White : PieceColor.Black;
      if (hand != "-")
      {
        int? multiplier = null;
        foreach (var ch in hand)
        {
          if (char.IsDigit(ch))
          {
            if (multiplier == null)
            {
              multiplier = int.Parse(ch.ToString());
            }
            else multiplier = multiplier*10 + int.Parse(ch.ToString());
          }
          else
          {
            IPieceType pieceType;
            if (!PT.TryParse(char.ToUpper(ch).ToString(), out pieceType))
              throw new ArgumentOutOfRangeException("sfenString", "Piece type is not found: " + ch);

            if (multiplier == null)
              multiplier = 1;
            for (int i = 0; i < multiplier; i++)
              (char.IsUpper(ch) ? res._blackHand : res._whiteHand).Add(pieceType);
            multiplier = null;
          }
        }
        res._whiteHand.Sort();
        res._blackHand.Sort();
      }
      return res;
    }
    /// <summary>Gets a SFEN representation of the board state</summary>
    public string ToSfenString()
    {
      var sb = new StringBuilder();
      for (int rankIndex = 0; rankIndex < 9; rankIndex++)
      {
        int gapSize = 0;
        for (int fileIndex = 0; fileIndex < 9; fileIndex++)
        {
          var piece = _cells[8 - fileIndex, rankIndex];
          if (piece == null)
          {
            gapSize++;
          }
          else
          {
            if (gapSize > 0)
            {
              sb.Append(gapSize);
              gapSize = 0;
            }
            var symbol = piece.PieceType.PieceKind.ToString();
            if (piece.Color == PieceColor.White)
              symbol = symbol.ToLower();
            if (piece.PieceType.IsPromoted)
              sb.Append("+");
            sb.Append(symbol);
          }
        }
        if (gapSize > 0)
          sb.Append(gapSize);
        sb.Append("/");
      }
      sb.Remove(sb.Length - 1, 1);
      sb.Append(" ");
      sb.Append(SideOnMove == PieceColor.White ? "W" : "B");
      sb.Append(" ");
      if (BlackHand.Count + WhiteHand.Count == 0)
      {
        sb.Append("-");
      }
      foreach (var pieceTypeGroup in BlackHand.GroupBy(pt => pt))
      {
        var groupSize = pieceTypeGroup.Count();
        if (groupSize > 1)
          sb.Append(groupSize);
        sb.Append(pieceTypeGroup.Key.Latin);
      }
      foreach (var pieceTypeGroup in WhiteHand.GroupBy(pt => pt))
      {
        var groupSize = pieceTypeGroup.Count();
        if (groupSize > 1)
          sb.Append(groupSize);
        sb.Append(pieceTypeGroup.Key.Latin.ToLower());
      }
      sb.Append(" ");
      sb.Append(Move == null ? 1 : Move.Number + 1);
      return sb.ToString();
    }

    /// <summary>Whether the same game position occurs four times with the same player to play
    /// (the game is considered a draw)</summary>
    public bool IsDrawByRepitition()
    {
      return KnownPositions.CountSimilar(this) >= 4;
    }

    private BoardSnapshot()
    {
      GameState = ShogiGameState.NotDefined;
    }
    private BoardSnapshot(List<IPieceType> blackHand, List<IPieceType> whiteHand)
      : this()
    {
      _blackHand = blackHand;
      _whiteHand = whiteHand;
    }

    /// <summary>ctor</summary>
    public BoardSnapshot(PieceColor sideOnMove,
      IEnumerable<Tuple<Position, IColoredPiece>> boardPieces,
      IEnumerable<IPieceType> whiteHand = null,
      IEnumerable<IPieceType> blackHand = null)
      : this()

    {
      if (boardPieces == null) throw new ArgumentNullException("boardPieces");

      SideOnMove = sideOnMove;
      foreach (var t in boardPieces)
        SetPiece(t.Item1, t.Item2);

      _whiteHand = whiteHand != null ? whiteHand.ToList() : EmptyList<IPieceType>.Instance;
      _whiteHand.Sort();
      _blackHand = blackHand != null ? blackHand.ToList() : EmptyList<IPieceType>.Instance; 
      _blackHand.Sort();
    }
    
    /// <summary>Creates a snapshot of the board with applied <paramref name="move"/></summary>
    public BoardSnapshot MakeMove(Move move)
    {
      if (move == null) throw new ArgumentNullException("move");

      // TODO: move.RulesViolation == RulesViolation.PartiallyValidated is a hack!
      if (move.RulesViolation != RulesViolation.PartiallyValidated &&
          move.RulesViolation != RulesViolation.ValidationInProgress)
      {
        if (move.BoardSnapshotBefore != this)
          throw new ArgumentException(
            "You cannot apply this move to the given board because it " +
            "is different from the one move had been created for");
        if (!move.IsValid)
          throw new InvalidMoveException(move.RulesViolation);
      }
      return new BoardSnapshot(this, move);
    }
    /// <summary>Gets the piece snapshot at the <paramref name="position"/></summary>
    public IColoredPiece GetPieceAt(Position position)
    {
      return _cells[position.X, position.Y]; 
    }

    /// <summary>Gets the hand collection by color</summary>
    public ReadOnlyCollection<IPieceType> GetHand(PieceColor color)
    {
      return color == PieceColor.White ? WhiteHand : BlackHand;
    }
    /// <summary>Checks wheter it's mate for <paramref name="color"/> king </summary>
    public bool IsMateFor(PieceColor color)
    {
      if (GameState == ShogiGameState.NotDefined)
      {
        if (IsCheckFor(color))
        {
          if (DoesntHaveValidMoves(color))
          {
            GameState = color == PieceColor.White 
              ? ShogiGameState.BlackWin 
              : ShogiGameState.WhiteWin;
          }
          else
          {
            GameState = ShogiGameState.None;
          }
        }
        else if (IsCheckFor(Opponent(color)))
        {
          if (DoesntHaveValidMoves(Opponent(color)))
          {
            GameState = Opponent(color) == PieceColor.White
              ? ShogiGameState.BlackWin
              : ShogiGameState.WhiteWin;
          }
          else
          {
            GameState = ShogiGameState.None;
          }
        }
      }
      return (color == PieceColor.White 
          && GameState == ShogiGameState.BlackWin) ||
          (color == PieceColor.Black &&
          GameState == ShogiGameState.WhiteWin);
    }
    /// <summary>Checks wheter it's check for <paramref name="color"/> king </summary>
    public bool IsCheckFor(PieceColor color)
    {
      var king = FindTheKing(color);
      return king != null &&
        GetAllValidUsualMovesDontCheckTheCheck(Opponent(color)).
        Any(move => move.To == king);
    }
    /// <summary>Gets all valid usual moves from the position</summary>
    public IEnumerable<UsualMove> GetAvailableUsualMoves(Position fromPosition)
    {
      var estimate = from p in EstimateUsualMoveTargets(fromPosition)
                     select new UsualMove(this, GetPieceAt(fromPosition).Color, fromPosition, p, false);
      return from move in DuplicateForPromoting(estimate)
             where ValidateUsualMoveHard(move) == RulesViolation.NoViolations
             select MarkValid(move);
    }
    /// <summary>Gets all valid drop moves for the piece</summary>
    public IEnumerable<DropMove> GetAvailableDropMoves(IPieceType piece, PieceColor color)
    {
      return Position.OnBoard.Where(p => GetPieceAt(p) == null).
        Select(p => new DropMove(this, piece.GetColored(color), p)).
        Where(move => move.IsValid);
    }
    /// <summary>Gets all valid usual and drop moves for the player</summary>
    public IEnumerable<Move> GetAvailableMoves(PieceColor color)
    {
      foreach (var move in GetAllAvailableUsualMoves(color))
        yield return move;
      foreach (var move in GetAllValidDropMoves(color))
        yield return move;
    }
    /// <summary>Gets a string representation of the board</summary>
    /// <returns>SFEN</returns>
    public override string ToString()
    {
      return ToSfenString();
    }

    #endregion

    #region ' Internal Interface '

    internal List<IPieceType> GetHandCollection(PieceColor color)
    {
      return color == PieceColor.White ? _whiteHand : _blackHand;
    }
    internal RulesViolation ValidateDropMove(DropMove move)
    {
      if (move == null) throw new ArgumentNullException("move");
      
      move.CheckIsNotValidatedAtAll();

      if (move.Piece.Color != SideOnMove) return RulesViolation.WrongSideToMove;
      if (!GetHand(SideOnMove).Contains(move.Piece.PieceType)) return RulesViolation.WrongPieceReference;
      if (GetPieceAt(move.To) != null) return RulesViolation.DropToOccupiedCell;

      if (move.Piece.PieceType == PT.歩 || move.Piece.PieceType == PT.香)
        if (HowFarFromTheLastLine(move.Piece, move.To) == 0)
          return RulesViolation.DropToLastLines;

      if (move.Piece.PieceType == PT.桂)
        if (HowFarFromTheLastLine(move.Piece, move.To) < 2)
          return RulesViolation.DropToLastLines;

      if (move.Piece.PieceType == PT.歩)
        if (IsTherePawnOnThisColumn(SideOnMove, move.To.X))
          return RulesViolation.TwoPawnsOnTheSameFile;

      move.MarkPartiallyValid();

      if (move.Piece.PieceType == PT.歩)
        if (move.BoardSnapshotAfter.IsMateFor(Opponent(SideOnMove)))
          return RulesViolation.DropPawnToMate;

      if (move.BoardSnapshotAfter.IsCheckFor(SideOnMove)) 
        return RulesViolation.MoveToCheck;// TODO: Not tested!

      if (KnownPositions.CountSimilar(move.BoardSnapshotAfter) >= 4)
        if (move.BoardSnapshotAfter.IsCheckFor(Opponent(SideOnMove)))
          return RulesViolation.PerpetualCheck;

      return RulesViolation.NoViolations;
    }
    internal RulesViolation ValidateUsualMove(UsualMove move)
    {
      if (move == null) throw new ArgumentNullException("move");

      move.CheckIsNotValidatedAtAll();

      var movingPiece = GetPieceAt(move.From);

      if (movingPiece == null)
        return RulesViolation.WrongPieceReference;

      if (move.From == move.To)
        return RulesViolation.PieceDoesntMoveThisWay;

      if (movingPiece.Color != SideOnMove)
        return RulesViolation.WrongSideToMove;

      var takenPiece = GetPieceAt(move.To);
      if (takenPiece != null)
        if (movingPiece.Color == takenPiece.Color)
          return RulesViolation.TakeAllyPiece;

      var analyser = EstimateUsualMoveTargets(move.From);
      if (!analyser.Contains(move.To))
        return RulesViolation.PieceDoesntMoveThisWay;

      return ValidateUsualMoveHard(move);
    }
    internal void SetPiece(Position position, IColoredPiece value)
    {
      _cells[position.X, position.Y] = value;
    }

    #endregion

    #region ' Implemetation '

    private BoardSnapshot(BoardSnapshot board, Move move)
      : this()
    {
      Move = move;
      SideOnMove = board.SideOnMove;

      foreach (var p in Position.OnBoard)
        SetPiece(p, board.GetPieceAt(p));

      _whiteHand = board.WhiteHand.ToList();
      _whiteHand.Sort();
      _blackHand = board.BlackHand.ToList();
      _blackHand.Sort();

      Move.Apply(this);
      SideOnMove = Opponent(SideOnMove);
    }

    private IEnumerable<UsualMove> GetAllAvailableUsualMoves(PieceColor color)
    {
      return Position.OnBoard.Where(p => GetPieceAt(p) != null && GetPieceAt(p).Color == color).
        SelectMany(p => DuplicateForPromoting(GetAvailableUsualMoves(p)));
    }
    private IEnumerable<UsualMove> GetAllValidUsualMovesDontCheckTheCheck(PieceColor color)
    {
      return Position.OnBoard.Where(p => GetPieceAt(p) != null && GetPieceAt(p).Color == color).
        SelectMany(GetAllValidUsualMovesDontCheckTheCheck);
    }
    private IEnumerable<UsualMove> GetAllValidUsualMovesDontCheckTheCheck(Position f)
    {
      return from p in EstimateUsualMoveTargets(f)
             select new UsualMove(this, GetPieceAt(f).Color, f, p, false);
    }
    private IEnumerable<DropMove> GetAllValidDropMoves(PieceColor color)
    {
      return GetHand(color).Distinct().SelectMany(p => GetAvailableDropMoves(p, color));
    }

    private bool DoesntHaveValidMoves(PieceColor color)
    {
      return GetAvailableMoves(color).FirstOrDefault() == null;
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
          yield return new UsualMove(this, GetPieceAt(m.From).Color, m.From, m.To, false);

        if (IsPromotionAllowed(GetPieceAt(m.From), m.From, m.To) == RulesViolation.NoViolations)
          yield return new UsualMove(this, GetPieceAt(m.From).Color, m.From, m.To, true);
      }
    }
    private static PieceColor Opponent(PieceColor color)
    {
      return color == PieceColor.White ? PieceColor.Black : PieceColor.White;
    }
    private bool IsTherePawnOnThisColumn(PieceColor color, int column)
    {
      for (var i = 0; i < 9; i++)
      {
        var piece = _cells[column, i];

        if (piece != null)
          if (piece.PieceType == PT.歩)
            if (piece.Color == color)
              return true;
      }

      return false;
    }
    private RulesViolation ValidateUsualMoveHard(UsualMove move)
    {
      move.CheckIsNotValidatedAtAll();

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

      move.MarkPartiallyValid();
      return !move.BoardSnapshotAfter.IsCheckFor(move.Who) 
        ? RulesViolation.NoViolations 
        // TODO: Not tested!
        : RulesViolation.MoveToCheck;
    }

    private int CalculateHashCode()
    {
      return EnumerableExtensions.GetSeqHashCode(
        SideOnMove.GetHashCode(),
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
             Equals(other.SideOnMove, SideOnMove) &&
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

    #region ' Promotion '

    /// <summary>Returns distance for from the last line for the piece (of the color)</summary>
    private static int HowFarFromTheLastLine(IColoredPiece coloredPiece, Position position)
    {
      var lastLineIndex = coloredPiece.Color == PieceColor.White ? 8 : 0;
      return Math.Abs(position.Y - lastLineIndex);
    }
    /// <summary>Returns null it the piece can move to the 
    ///   <paramref name="position"/> without promotion -or- 
    ///   text with explanation why he's not allowed to do that</summary>
    private static RulesViolation IsPromotionMandatory(IColoredPiece coloredPiece, Position position)
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
    private static RulesViolation IsPromotionAllowed(IColoredPiece coloredPiece, Position from, Position to)
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

    private static bool IsThatPromitionZoneFor(IColoredPiece coloredPiece, Position position)
    {
      return HowFarFromTheLastLine(coloredPiece, position) < 3;
    }

    #endregion

    private static T MarkValid<T>(T move)
      where T : Move
    {
      move.MarkValid();
      return move;
    }

    #region ' Nested classes '

    [Serializable]
    private class KnownPositions : Dictionary<BoardSnapshot, KnownPositions.Integer>
    {
      private const int Arity = 10;

      [Serializable]
      public class Integer
      {
        public int Value { get; set; }
      }

      private KnownPositions()
      {
      }

      private KnownPositions(IDictionary<BoardSnapshot, Integer> dictionary)
        : base(dictionary)
      {
      }

      private static KnownPositions GetAll(BoardSnapshot current)
      {
        var dic = GetNearestKnownPositionsDic(current);
        var res = dic == null ? new KnownPositions() : new KnownPositions(dic);

        while (true)
        {
          var move = current.Move;
          if (move == null) break;

          {
            Integer counter;
            if (!res.TryGetValue(current, out counter))
              res[current] = counter = new Integer();

            counter.Value++;
          }

          current = move.BoardSnapshotBefore;
          if (current == null) break;
          if (current._knownPositions == null) break;
        }

        return res;

      }
      private static KnownPositions GetNearestKnownPositionsDic(BoardSnapshot current)
      {
        while (true)
        {
          var move = current.Move;
          if (move == null) return null;
          current = move.BoardSnapshotBefore;
          if (current == null) return null;
          if (current._knownPositions != null) return current._knownPositions;
        }
      }
      public static int CountSimilar(BoardSnapshot position)
      {
        var m = position.Move;
        if (m == null) return 0;
        var current = m.BoardSnapshotBefore;

        var counter = 0;
        while (true)
        {
          if (current._knownPositions != null) break;

          var move = current.Move;
          if (move != null && move.Number % Arity == 0)
          {
            current._knownPositions = GetAll(current);
            break;
          }

          if (current == position)
          {
            counter++;
          }
          current = move == null ? null : move.BoardSnapshotBefore;

          if (current == null) break;
        }

        if (current == null)
          return 0;
        Integer res;
        var rres = current._knownPositions.TryGetValue(position, out res) ? res.Value : 0;
        return rres + counter;
      }
    }

    #endregion
  }
}
// TODO: All constructors call : this()?