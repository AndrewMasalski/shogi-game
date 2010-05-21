using System;
using System.Collections.Generic;
using System.Linq;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.Primitives;
using Yasc.ShogiCore.Snapshots;

namespace Yasc.ShogiCore.PieceSets
{
  /// <summary>40 pieces</summary>
  public class StandardPieceSet : IPieceSet
  {
    private readonly List<Piece>[] _set;

    /// <summary>ctor</summary>
    public StandardPieceSet()
    {
      _set = new List<Piece>[9];
      for (var i = 0; i < _set.Length; i++)
        _set[i] = new List<Piece>();

      var allPiecesInInitialPosition =
        from position in Position.OnBoard
        select BoardSnapshot.InitialPosition[position]
        into pieceSnapshot
        where pieceSnapshot != null
        select pieceSnapshot.PieceType;

      foreach (var pieceType in allPiecesInInitialPosition)
        _set[pieceType.Id].Add(new Piece(pieceType));
    }

    /// <summary>Gets reference to the piece from the set by type</summary>
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
    /// <summary>Marks given <paramref name="piece"/> as occupied</summary>
    public void AcquirePiece(Piece piece)
    {
      if (piece == null) throw new ArgumentNullException("piece");
      var list = _set[piece.PieceType.Id];
      if (!list.Remove(piece))
      {
        throw new InvalidOperationException(
          "Cannot take the piece because it has already been taken");
      }
    }
    /// <summary>Returns given <paramref name="piece"/> to the set</summary>
    public void ReleasePiece(Piece piece)
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