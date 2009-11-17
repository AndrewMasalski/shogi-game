using System;
using System.Collections;
using System.Collections.Generic;
using Yasc.ShogiCore.Utils;
using Yasc.Utils;

namespace Yasc.ShogiCore
{
  public class AvailableUsualMovesAnalyser : IEnumerable<Position>
  {
    private readonly BoardSnapshot _board;
    private readonly Position _from;
    private Vector UpDirection
    {
      get { return _board[_from].Color == PieceColor.White ? new Vector(1, 1) : new Vector(1, -1); }
    }

    private const int Max = 8;

    public AvailableUsualMovesAnalyser(BoardSnapshot board, Position from)
    {
      if (board == null) throw new ArgumentNullException("board");
      _board = board;
      _from = from;
    }
    public IEnumerator<Position> GetEnumerator()
    {
      return Switch().GetEnumerator();
    }
    private IEnumerable<Position> Switch()
    {
      switch ((string)_board[_from].Type)
      {
        case "玉": return GetMovesFor玉();
        case "飛": return GetMovesFor飛();
        case "角": return GetMovesFor角();
        case "金": return GetMovesFor金();
        case "銀": return GetMovesFor銀();
        case "桂": return GetMovesFor桂();
        case "香": return GetMovesFor香();
        case "歩": return GetMovesFor歩();
        case "竜": return GetMovesFor竜();
        case "馬": return GetMovesFor馬();
        case "全": return GetMovesFor金();
        case "今": return GetMovesFor金();
        case "仝": return GetMovesFor金();
        case "と": return GetMovesFor金();
      }
      throw new Exception();
    }

    private IEnumerable<Position> GetMovesFor馬()
    {
      return Join(GetMovesFor角(), Up(1), Right(1), Down(1), Left(1));
    }
    private IEnumerable<Position> GetMovesFor竜()
    {
      return Join(GetMovesFor飛(), UpLeft(1), UpRight(1), DownRight(1), DownLeft(1));
    }
    private IEnumerable<Position> GetMovesFor歩()
    {
      return Up(1);
    }
    private IEnumerable<Position> GetMovesFor香()
    {
      return Up(Max);
    }
    private IEnumerable<Position> GetMovesFor桂()
    {
      return Join(Go(1, 2, 1), Go(-1, 2, 1));
    }
    private IEnumerable<Position> GetMovesFor銀()
    {
      return Join(Up(1), UpRight(1), DownRight(1), DownLeft(1), UpLeft(1));
    }
    private IEnumerable<Position> GetMovesFor金()
    {
      return Join(Up(1), UpRight(1), Right(1), Down(1), Left(1), UpLeft(1));
    }
    private IEnumerable<Position> GetMovesFor角()
    {
      return Join(UpRight(Max), DownRight(Max), DownLeft(Max), UpLeft(Max));
    }
    private IEnumerable<Position> GetMovesFor飛()
    {
      return Join(Up(Max), Right(Max), Down(Max), Left(Max));
    }
    private IEnumerable<Position> GetMovesFor玉()
    {
      return Join(Up(1), UpRight(1), Right(1), DownRight(1), Down(1), DownLeft(1), Left(1), UpLeft(1));
    }

    private IEnumerable<Position> Up(int i)
    {
      return Go(0, 1, i);
    }
    private IEnumerable<Position> UpRight(int i)
    {
      return Go(1, 1, i);
    }
    private IEnumerable<Position> Right(int i)
    {
      return Go(1, 0, i);
    }
    private IEnumerable<Position> DownRight(int i)
    {
      return Go(1, -1, i);
    }
    private IEnumerable<Position> Down(int i)
    {
      return Go(0, -1, i);
    }
    private IEnumerable<Position> DownLeft(int i)
    {
      return Go(-1, -1, i);
    }
    private IEnumerable<Position> Left(int i)
    {
      return Go(-1, 0, i);
    }
    private IEnumerable<Position> UpLeft(int i)
    {
      return Go(-1, 1, i);
    }

    private IEnumerable<Position> Go(int dx, int dy, int count)
    {
      var delta = new Vector(dx, dy) * UpDirection;
      for (var curr = _from + delta; count > 0; count--, curr += delta)
      {
        if (!curr.IsValidPosition)
        {
          yield break;
        }
        if (_board[curr] == null)
        {
          yield return curr;
        }
        else if (_board[curr].Color != MyColor)
        {
          yield return curr;
          yield break;
        }
        else yield break;
      }
    }

    private PieceColor MyColor
    {
      get { return _board[_from].Color; }
    }

    private static IEnumerable<T> Join<T>(params IEnumerable<T>[] arr)
    {
      foreach (var enumerable in arr)
        foreach (var item in enumerable)
          yield return item;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}