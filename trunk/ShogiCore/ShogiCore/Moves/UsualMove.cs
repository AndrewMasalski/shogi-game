using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yasc.ShogiCore.Snapshots;

namespace Yasc.ShogiCore.Moves
{
  /// <summary>Represents usual move (as opposing to the <see cref="DropMove"/>)</summary>
  public sealed class UsualMove : MoveBase
  {
    private string _cuteNotation;

    /// <summary>Move origin</summary>
    public Position From { get; private set; }
    /// <summary>Move target</summary>
    public Position To { get; private set; }
    /// <summary>Piece being moved</summary>
    public Piece MovingPiece { get; private set; }
    /// <summary>Piece being taken by the move</summary>
    public Piece TakenPiece { get; private set; }
    /// <summary>Indicates whether move is promoting</summary>
    public bool IsPromoting { get; private set; }

    /// <summary>Returns snapshot of the move</summary>
    public override MoveSnapshotBase Snapshot()
    {
      return new UsualMoveSnapshot(Who.Color, From, To, IsPromoting);
    }
    /// <summary>Gets user friendly move transcription (latin)</summary>
    public override string ToString()
    {
      return From + "-" + To + (IsPromoting ? "+" : "");
    }

    #region ' CuteNotation '
    
    /// <summary>Move transcript in "cute" notation</summary>
    public string CuteNotation
    {
      get { return _cuteNotation ?? (_cuteNotation = GetCuteNotation()); }
    }
    private string GetCuteNotation()
    {
      var sb = new StringBuilder();
      sb.Append(MovingPiece.PieceType);
      var hint = GetHint(WhoElseCouldMoveThere());
      if (hint != null)
      {
        sb.Append(hint);
        if (TakenPiece == null)
          sb.Append("-");
      }
      if (TakenPiece != null)
        sb.Append("x");
      sb.Append(To);
      return sb.ToString();
    }
    private string GetHint(IEnumerable<Position> list)
    {
      var s = new HashSet<char>(From.ToString());
      foreach (var position in list)
        foreach (var c in position.ToString())
          s.Remove(c);

      if (s.Count == 0)
      {
        return From.ToString();
      }
      if (s.Count == 1)
      {
        return s.First().ToString();
      }
      return null;
    }
    private IEnumerable<Position> WhoElseCouldMoveThere()
    {
      return from p in Position.OnBoard 
             where p != From 
             where BoardSnapshot[p] != null
             where BoardSnapshot[p].PieceType == MovingPiece.PieceType
             where BoardSnapshot[p].Color == Who.Color 
             where BoardSnapshot.GetAvailableUsualMoves(p).Count(m => m.To == To) > 0 
             select p;
    }

    #endregion

    #region ' Construction '

    private UsualMove(Board board, Position from, Position to, bool isPromoting)
      : base(board, board.OneWhoMoves)
    {
      From = from;
      To = to;
      IsPromoting = isPromoting;
      MovingPiece = board[from];
      TakenPiece = board[to];
    }

    /// <summary>Creates an instance of <see cref="UsualMove"/> 
    ///   from origin and target positions and validates it immediately</summary>
    public static UsualMove Create(Board board, Position from, Position to, bool isPromoting)
    {
      var res = new UsualMove(board, from, to, isPromoting);
      res.Validate();
      return res;
    }
    /// <summary>Creates an instance of <see cref="UsualMove"/> 
    ///   from snapshot and validates it immediately</summary>
    public static UsualMove Create(Board board, UsualMoveSnapshot snapshot)
    {
      var res = new UsualMove(board, snapshot.From, snapshot.To, snapshot.IsPromoting);
      res.Validate();
      return res;
    }

    #endregion

    /// <summary>Override to get validation error message or null if move is valid</summary>
    protected override string GetValidationErrorMessage()
    {
      return BoardSnapshot.ValidateUsualMove(
        new UsualMoveSnapshot(Who.Color, From, To, IsPromoting));
    }
    /// <summary>Apply the move to the board</summary>
    protected internal override void Make()
    {
      var piece = Board[From];
      Board.ResetPiece(From);
      if (IsPromoting) piece.IsPromoted = true;
      var targetPiece = Board[To];
      if (targetPiece != null)
      {
        Board.ResetPiece(To);
        Who.Hand.Add(targetPiece);
      }
      Board.SetPiece(piece, Who, To);
    }
  }
}