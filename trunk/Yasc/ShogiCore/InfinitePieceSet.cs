using System;
using System.Collections.Generic;

namespace Yasc.ShogiCore
{
  internal class InfinitePieceSet : IPieceSet
  {
    private readonly List<Piece>[] _set;

    internal InfinitePieceSet()
    {
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
          list.Add(new Piece(type));
        }

        var last = list[list.Count - 1];
        last.IsPromoted = type.IsPromoted;
        return last;
      }
    }
    public void Pop(Piece piece)
    {
      if (piece == null) throw new ArgumentNullException("piece");
      var list = _set[piece.PieceType.Id];
      if (!list.Remove(piece))
      {
        throw new InvalidOperationException(
          "Cannont take the piece because it has already been taken");
      }
    }
    public void Push(Piece piece)
    {
      if (piece == null) throw new ArgumentNullException("piece");
      piece.Owner = null;
      var list = _set[piece.PieceType.Id];
      if (list.Contains(piece))
      {
        throw new InvalidOperationException(
          "Cannot return piece which is already in the set");
      }
      list.Add(piece);
    }
  }
}