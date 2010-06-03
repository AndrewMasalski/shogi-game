using System;
using System.Collections.Generic;
using System.Linq;
using Yasc.ShogiCore.Primitives;
using Yasc.ShogiCore.Snapshots;
using Yasc.Utils;

namespace Yasc.ShogiCore.Notations
{
  /// <summary>Can parse cute move notation like "+Rx1a"</summary>
  public class CuteNotation : Singletone<CuteNotation>, INotation
  {
    private BoardSnapshot _board;
    private string _moveText;

    /// <summary>Gets move on the board parsing it from transcript</summary>
    /// <param name="originalBoardState">State of the board before move</param>
    /// <param name="move">Move trancsript to parse</param>
    /// <returns>All moves which may be transcribed given way. 
    ///   Doesn't return null but be prepared to receive 0 moves.</returns>
    public IEnumerable<Move> Parse(BoardSnapshot originalBoardState, string move)
    {
      if (move == "resign") return new[] {
        new ResignMove(originalBoardState, originalBoardState.SideOnMove) };

      _board = originalBoardState;
      _moveText = move;
      return Parse();
    }

    private IEnumerable<Move> Parse()
    {
      return _moveText.Contains("x") ? ParseTake() : ParseMove();
    }

    private IEnumerable<Move> ParseMove()
    {
      _moveText = _moveText.Replace("x", ""); // TODO: remove line
      var pieceType = GetPieceType();
      var isPromoting = GetIsPromoting();
      var toPosition = GetToPosition();
      var fromPosition = FindFromPosition(_moveText, pieceType, Position.Parse(toPosition), isPromoting);
      return CreateMoves(pieceType, toPosition, isPromoting, fromPosition);
    }
    private IEnumerable<Move> CreateMoves(IPieceType pieceType, string toPosition, bool isPromoting, ICollection<Position> fromPositions)
    {
      return fromPositions.Count == 0 
        ? CreateDropMoves(pieceType, toPosition) 
        : CreateUsualMoves(fromPositions, toPosition, isPromoting);
    }

    private IEnumerable<Move> CreateUsualMoves(IEnumerable<Position> fromPositions, string toPosition, bool isPromoting)
    {
      return fromPositions.Select(
        fromPosition => new UsualMove(
          _board, 
          _board.GetPieceAt(fromPosition).Color, 
          fromPosition, 
          Position.Parse(toPosition), 
          isPromoting)).
        Where(move => move.IsValid);
    }

    private IEnumerable<DropMove> CreateDropMoves(IPieceType pieceType, string toPosition)
    {
      var move = new DropMove(_board, 
        pieceType.GetColored(_board.SideOnMove), 
        Position.Parse(toPosition));

      if (move.IsValid) yield return move;
    }

    private string GetToPosition()
    {
      var toPosition = _moveText.Substring(_moveText.Length - 2, 2);
      _moveText = _moveText.Substring(0, _moveText.Length - 2);
      return toPosition;
    }
    private bool GetIsPromoting()
    {
      var isPromoting = _moveText.EndsWith("+");
      _moveText = _moveText.TrimEnd('+');
      _moveText = _moveText.TrimEnd('=');
      return isPromoting;
    }
    private IPieceType GetPieceType()
    {
      var pieceTypeLength = _moveText.StartsWith("+") ? 2 : 1;
      var pieceTypeStr = _moveText.Substring(0, pieceTypeLength);
      var pieceType = pieceTypeStr == "K" ? CurrentKing : pieceTypeStr;
      _moveText = _moveText.Substring(pieceTypeLength, _moveText.Length - pieceTypeLength);
      return PT.Parse(pieceType);
    }
    private IEnumerable<UsualMove> ParseTake()
    {
      if (_moveText.StartsWith("x"))
      {
        _moveText = _moveText.TrimStart('x');
        return ParseTakeByWhoTakes();
      }
      return ParseTakeByWhoTakes();
    }

    private IEnumerable<UsualMove> ParseTakeByWhoTakes()
    {
      var isPromoting = _moveText.EndsWith("+");
      var pieceType = GetPieceType();
      return from p in Position.OnBoard
             let piece = _board.GetPieceAt(p)
             where piece != null &&
                   piece.PieceType == pieceType &&
                   piece.Color == _board.SideOnMove
             from m in _board.GetAvailableUsualMoves(p)
             where m.IsPromoting == isPromoting
                   && _board.GetPieceAt(m.To) != null
                   && m.IsValid
             select m;
    }

  
    private string CurrentKing
    {
      // NOTE: Strictly speaking king type doesnt depend on color...
      get { return _board.SideOnMove == PieceColor.White ? "Kr" : "Kc"; }
    }
    private Position[] FindFromPosition(string hint, IPieceType pieceType, Position toPosition, bool isPromoting)
    {
      if (hint.Length == 3 && hint.EndsWith("-"))
      {
        var position = Position.Parse(hint.Substring(0, 2));
        if (_board.GetPieceAt(position) != null)
          return new[] { position };
        hint = "";
      }
      var candidates = (from p in Position.OnBoard
                        where _board.GetPieceAt(p) != null &&
                              _board.GetPieceAt(p).PieceType == pieceType &&
                              _board.GetPieceAt(p).Color == _board.SideOnMove &&
                              (from move in _board.GetAvailableUsualMoves(p)
                               where move.IsPromoting == isPromoting
                               select move.To).Contains(toPosition)
                        select p).ToArray();


      if (hint.Length == 0) return candidates;
      var result = candidates.Where(c => c.ToString().Contains(hint)).ToArray();
      // if we have a hint and it suits all choices we're parsing something wrong
      return result.Length == candidates.Length ? new Position[0] : result;
    }
    
    /// <summary>Returns the transcript for a given move</summary>
    /// <param name="originalBoardState">State of the board before move</param>
    /// <param name="move">Move to trancsript</param>
    public string ToString(BoardSnapshot originalBoardState, Move move)
    {
      throw new NotImplementedException();
    /*  var sb = new StringBuilder();
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
      return sb.ToString();*/
    }
  }
}