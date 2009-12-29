using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Yasc.ShogiCore.Snapshots;

namespace Yasc.ShogiCore.Moves
{
  public class UsualMove : MoveBase
  {
    private string _cuteNotation;

    public Position From { get; private set; }
    public Position To { get; private set; }
    public Piece MovingPiece { get; private set; }
    public Piece TakenPiece { get; private set; }
    public bool IsPromoting { get; private set; }

    public override MoveSnapshotBase Snapshot()
    {
      return new UsualMoveSnapshot(this);
    }
    public override string ToString()
    {
      return From + "-" + To + (IsPromoting ? "+" : "");
    }

    #region ' CuteNotation '

    public string CuteNotation
    {
      get
      {
        if (_cuteNotation == null)
        {
          _cuteNotation = GetCuteNotation();
        }
        return _cuteNotation;
      }
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
      /*  return from p in Position.OnBoard
               where p != From &&
                     Board[p] != null &&
                     Board[p].PieceType == MovingPiece.PieceType &&
                     Board[p].Color == Who.Color &&
                     Board.GetAvailableMoves(p).Count(m => m.To == To) > 0
               select p;*/
      foreach (var p in Position.OnBoard)
        if (p != From)
          if (BoardSnapshot[p] != null)
            if (BoardSnapshot[p].PieceType == MovingPiece.PieceType)
              if (BoardSnapshot[p].Color == Who.Color)
                if (BoardSnapshot.GetAvailableUsualMoves(p).Count(m => m.To == To) > 0)
                  yield return p;
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

    public static UsualMove Create(Board board, Position from, Position to, bool isPromoting)
    {
      var res = new UsualMove(board, from, to, isPromoting);
      res.Validate();
      return res;
    }
    public static UsualMove Create(Board board, UsualMoveSnapshot snapshot)
    {
      var res = new UsualMove(board, snapshot.From, snapshot.To, snapshot.IsPromoting);
      res.Validate();
      return res;
    }

    #endregion

    protected override string GetErrorMessage()
    {
      return BoardSnapshot.ValidateUsualMove(new UsualMoveSnapshot(this));
    }
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