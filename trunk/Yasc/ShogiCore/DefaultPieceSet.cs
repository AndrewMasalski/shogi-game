using System;
using System.Collections.Generic;

namespace Yasc.ShogiCore
{
  internal class DefaultPieceSet : IPieceSet
  {
    private readonly List<Piece>[] _set;

    internal DefaultPieceSet()
    {
      _set = new List<Piece>[8];
      for (int i = 0; i < _set.Length; i++)
        _set[i] = new List<Piece>();

      foreach (var position in Shogi.InitialPosition)
      {
        PieceType pieceType = position.Value;
        _set[pieceType.Id].Add(new Piece(pieceType));
      }
    }

    public Piece this[PieceType type]
    {
      get
      {
        var list = _set[type.Id];
        if (list.Count == 0)
        {
          return null;
        }

        var last = list[list.Count - 1];
        last.IsPromoted = type.IsPromoted;
        return last;
      }
    }
    public void Take(Piece piece)
    {
      if (piece == null) throw new ArgumentNullException("piece");
      var list = _set[piece.Type.Id];
      if (!list.Remove(piece))
      {
        throw new InvalidOperationException(
          "Cannont take the piece because it has already been taken");
      }
    }
    public void Return(Piece piece)
    {
      if (piece == null) throw new ArgumentNullException("piece");
      piece.Owner = null;
      var list = _set[piece.Type.Id];
      list.Add(piece);
    }
  }
}