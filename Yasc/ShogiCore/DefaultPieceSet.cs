using System;
using System.Collections.Generic;

namespace Yasc.ShogiCore
{
  public class DefaultPieceSet : IPieceSet
  {
    private readonly List<Piece>[] _set;

    internal DefaultPieceSet(Board board)
    {
      _set = new List<Piece>[8];
      for (int i = 0; i < _set.Length; i++)
        _set[i] = new List<Piece>();

      foreach (var position in Shogi.InitialPosition)
      {
        PieceType pieceType = position.Value;
        _set[pieceType.Id].Add(new Piece(board.White, pieceType));
      }
    }

    public Piece this[PieceType type]
    {
      get
      {
        var list = _set[type.Id];
        if (list.Count == 0)
        {
          throw new InvalidOperationException(
            "There's no more spare pieces of type " + type);
        }

        var last = list[list.Count - 1];
        return last;
      }
    }
    public void Take(Piece piece)
    {
      var list = _set[piece.Type.Id];
      if (!list.Remove(piece))
      {
        throw new InvalidOperationException(
          "Cannont take the piece because it has already been taken");
      }
    }
    public void Return(Piece piece)
    {
      piece.Owner = null;
      var list = _set[piece.Type.Id];
      list.Add(piece);
    }
  }
  public class InfinitePieceSet : IPieceSet
  {
    private readonly Board _board;
    private readonly List<Piece>[] _set;

    internal InfinitePieceSet(Board board)
    {
      _board = board;
      _set = new List<Piece>[8];
      for (int i = 0; i < _set.Length; i++)
        _set[i] = new List<Piece>();
    }

    public Piece this[PieceType type]
    {
      get
      {
        var list = _set[type.Id];
        if (list.Count == 0)
        {
          list.Add(new Piece(_board.White, type));
        }

        var last = list[list.Count - 1];
        return last;
      }
    }
    public void Take(Piece piece)
    {
      var list = _set[piece.Type.Id];
      if (!list.Remove(piece))
      {
        throw new InvalidOperationException(
          "Cannont take the piece because it has already been taken");
      }
    }
    public void Return(Piece piece)
    {
      piece.Owner = null;
      var list = _set[piece.Type.Id];
      list.Add(piece);
    }
  }
}