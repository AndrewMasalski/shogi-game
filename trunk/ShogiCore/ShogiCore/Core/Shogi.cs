using System;
using System.Collections.Generic;
using Yasc.ShogiCore.Primitives;
using Yasc.Utils;

namespace Yasc.ShogiCore.Core
{
  /// <summary>Contains very common knowledge about shogi game</summary>
  public static class Shogi
  {
    /// <summary>Contains position-pieceType pairs of initial position</summary>
    public static readonly ReadOnlyDictionary<Position, string> InitialPosition = Pairs(new[]
          {
            "1a", "香", "9a", "香", "1i", "香", "9i", "香",
            "2a", "桂", "8a", "桂", "2i", "桂", "8i", "桂",
            "3a", "銀", "7a", "銀", "3i", "銀", "7i", "銀",
            "4a", "金", "6a", "金", "4i", "金", "6i", "金",
            "5a", "王", "5i", "玉", "2b", "角", "8h", "角",
            "8b", "飛", "2h", "飛", "1c", "歩", "2c", "歩",
            "3c", "歩", "4c", "歩", "5c", "歩", "6c", "歩",
            "7c", "歩", "8c", "歩", "9c", "歩", "1g", "歩",
            "2g", "歩", "3g", "歩", "4g", "歩", "5g", "歩",
            "6g", "歩", "7g", "歩", "8g", "歩", "9g", "歩",
          },
        s => (Position)s).ToReadOnlyDictionary(pair => pair.Key, pair => pair.Value);

    private static IEnumerable<KeyValuePair<TKey, TValue>> Pairs<TKey, TValue>(IEnumerable<TValue> list, Converter<TValue, TKey> firstConverter)
    {
      var i = 0;
      var first = default(TValue);
      foreach (var item in list)
      {
        if (++i % 2 == 1)
          first = item;
        else
          yield return new KeyValuePair<TKey, TValue>(firstConverter(first), item);
      }
    }

    /// <summary>Sets start position on the board</summary>
    public static void InitBoard(Board board)
    {
      if (board == null) throw new ArgumentNullException("board");
      foreach (var pair in InitialPosition)
      {
        var party = pair.Key.Y < 5 ? board.White : board.Black;
        board.SetPiece((PieceType)pair.Value, party, pair.Key);
      }
    }
  }
}