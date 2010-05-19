using System;
using System.Collections.Generic;
using System.Linq;
using Yasc.ShogiCore.Core;
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

    public IEnumerable<MoveSnapshotBase> Parse(BoardSnapshot originalBoardState, string move)
    {
      _board = originalBoardState;
      _moveText = move;
      return Parse();
    }

    private IEnumerable<MoveSnapshotBase> Parse()
    {
      return _moveText.EndsWith("x") 
          || _moveText.EndsWith("x+")
          || _moveText.EndsWith("x=") 
          ? ParseTake() : ParseMove();
    }

    private IEnumerable<MoveSnapshotBase> ParseMove()
    {
      _moveText = _moveText.Replace("x", "");
      var pieceType = GetPieceType();
      var isPromoting = GetIsPromoting();
      var toPosition = GetToPosition();
      var fromPosition = FindFromPosition(_moveText, pieceType, toPosition, isPromoting);
      return CreateMoves(pieceType, toPosition, isPromoting, fromPosition);
    }
    private IEnumerable<MoveSnapshotBase> CreateMoves(PieceType pieceType, string toPosition, bool isPromoting, ICollection<Position> fromPositions)
    {
      return fromPositions.Count == 0 
        ? CreateDropMoves(pieceType, toPosition) 
        : CreateUsualMoves(fromPositions, toPosition, isPromoting);
    }

    private IEnumerable<MoveSnapshotBase> CreateUsualMoves(IEnumerable<Position> fromPositions, string toPosition, bool isPromoting)
    {
      return fromPositions.Select(fromPosition => 
        new UsualMoveSnapshot(_board[fromPosition].Color, fromPosition, toPosition, isPromoting)).
        Where(move => _board.ValidateUsualMove(move) == null);
    }

    private IEnumerable<DropMoveSnapshot> CreateDropMoves(PieceType pieceType, string toPosition)
    {
      var dropMoveSnapshot = new DropMoveSnapshot(pieceType, _board.OneWhoMoves, toPosition);
      if (_board.ValidateDropMove(dropMoveSnapshot) == null)
        yield return dropMoveSnapshot;
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
    private PieceType GetPieceType()
    {
      var pieceTypeLength = _moveText.StartsWith("+") ? 2 : 1;
      var pieceTypeStr = _moveText.Substring(0, pieceTypeLength);
      var pieceType = pieceTypeStr == "K" ? CurrentKing : pieceTypeStr;
      _moveText = _moveText.Substring(pieceTypeLength, _moveText.Length - pieceTypeLength);
      return pieceType;
    }
    private IEnumerable<UsualMoveSnapshot> ParseTake()
    {
      var isPromoting = _moveText.EndsWith("+");
      var pieceType = GetPieceType();
      return from p in Position.OnBoard
             where _board[p] != null &&
                   _board[p].PieceType == pieceType &&
                   _board[p].Color == _board.OneWhoMoves
             from m in _board.GetAvailableUsualMoves(p)
             where m.IsPromoting == isPromoting 
                && _board[m.To] != null 
                && _board.ValidateUsualMove(m) == null
             select m;
    }
    protected string CurrentKing
    {
      // NOTE: Strictly speaking king type doesnt depend on color...
      get { return _board.OneWhoMoves == PieceColor.White ? "Kr" : "Kc"; }
    }
    private Position[] FindFromPosition(string hint, PieceType pieceType, Position toPosition, bool isPromoting)
    {
      var candidates = (from p in Position.OnBoard
                        where _board[p] != null &&
                              _board[p].PieceType == pieceType &&
                              _board[p].Color == _board.OneWhoMoves &&
                              (from move in _board.GetAvailableUsualMoves(p)
                               where move.IsPromoting == isPromoting
                               select move.To).Contains(toPosition)
                        select p).ToArray();


      if (hint.Length == 0) return candidates;
      var result = candidates.Where(c => c.ToString().Contains(hint)).ToArray();
      // if we have a hint and it suits all choices we're parsing something wrong
      return result.Length == candidates.Length ? new Position[0] : result;
    }


    public string ToString(BoardSnapshot originalBoardState, MoveSnapshotBase move)
    {
      throw new NotImplementedException();
    }
  }
}