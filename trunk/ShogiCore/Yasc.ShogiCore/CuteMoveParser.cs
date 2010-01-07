using System;
using System.Collections.Generic;
using System.Linq;
using Yasc.ShogiCore.Moves;

namespace Yasc.ShogiCore
{
  public class CuteMoveParser
  {
    private readonly Board _board;
    private string _moveText;

    public CuteMoveParser(Board board)
    {
      if (board == null) throw new ArgumentNullException("board");
      _board = board;
    }

    public IEnumerable<MoveBase> Parse(string moveText)
    {
      _moveText = moveText;
      IEnumerable<MoveBase> choice;
      if (_moveText.EndsWith("x") || _moveText.EndsWith("x+") || _moveText.EndsWith("x=")) 
        choice = ParseTake();
      else 
        choice = ParseMove();
      return from option in choice where option.IsValid select option;
    }

    private IEnumerable<MoveBase> ParseMove()
    {
      _moveText = _moveText.Replace("x", "");
      var pieceType = GetPieceType();
      bool isPromoting = GetIsPromoting();
      string toPosition = GetToPosition();
      var fromPosition = FindFromPosition(_moveText, pieceType, toPosition, isPromoting);
      return CreateMoves(pieceType, toPosition, isPromoting, fromPosition);
    }

    private IEnumerable<MoveBase> CreateMoves(PieceType pieceType, string toPosition, bool isPromoting, Position[] fromPosition)
    {
      if (fromPosition.Length == 0)
      {
        yield return _board.GetDropMove(pieceType, toPosition, _board.OneWhoMoves);
      }
      else
      {
        foreach (var option in fromPosition)
          yield return _board.GetUsualMove(option, toPosition, isPromoting);
      }
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
      int pieceTypeLength = _moveText.StartsWith("+") ? 2 : 1;
      string pieceTypeStr = _moveText.Substring(0, pieceTypeLength);
      var pieceType = pieceTypeStr == "K" ? CurrentKing : pieceTypeStr;
      _moveText = _moveText.Substring(pieceTypeLength, _moveText.Length - pieceTypeLength);
      return pieceType;
    }

    private IEnumerable<MoveBase> ParseTake()
    {
      var isPromoting = _moveText.EndsWith("+");
      var pieceType = GetPieceType();
      return from p in Position.OnBoard
             where _board[p] != null &&
                   _board[p].PieceType == pieceType &&
                   _board[p].Owner == _board.OneWhoMoves
             from m in _board.GetAvailableMoves(p)
             where m.IsPromoting == isPromoting && m.TakenPiece != null
             select (MoveBase)m;
    }

    protected string CurrentKing
    {
      // NOTE: Strictly speaking king type doesnt depend on color...
      get { return _board.OneWhoMoves.Color == PieceColor.White ? "Kr" : "Kc"; }
    }

    private Position[] FindFromPosition(string hint, PieceType pieceType, Position toPosition, bool isPromoting)
    {
      var candidates = (from p in Position.OnBoard
                        where _board[p] != null &&
                              _board[p].PieceType == pieceType &&
                              _board[p].Owner == _board.OneWhoMoves &&
                              (from move in _board.GetAvailableMoves(p)
                               where move.IsPromoting == isPromoting
                               select move.To).Contains(toPosition)
                        select p).ToArray();


      if (hint.Length == 0) return candidates;
      var result = candidates.Where(c => c.ToString().Contains(hint)).ToArray();
      // if we have a hint and it suits all choices we're parsing something wrong
      return result.Length == candidates.Length ? new Position[0] : result;
    }
  }
}