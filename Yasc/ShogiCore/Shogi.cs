using System;
using System.Collections.Generic;

namespace Yasc.ShogiCore
{
  public static class Shogi
  {
    public static readonly Dictionary<Position, string> InitialPosition = Dic(Pairs(new[]
          {
            "1a", "香", "9a", "香", "1i", "香", "9i", "香",
            "2a", "桂", "8a", "桂", "2i", "桂", "8i", "桂",
            "3a", "銀", "7a", "銀", "3i", "銀", "7i", "銀",
            "4a", "金", "6a", "金", "4i", "金", "6i", "金",
            "5a", "玉", "5i", "玉", "2b", "角", "8h", "角",
            "8b", "飛", "2h", "飛", "1c", "歩", "2c", "歩",
            "3c", "歩", "4c", "歩", "5c", "歩", "6c", "歩",
            "7c", "歩", "8c", "歩", "9c", "歩", "1g", "歩",
            "2g", "歩", "3g", "歩", "4g", "歩", "5g", "歩",
            "6g", "歩", "7g", "歩", "8g", "歩", "9g", "歩",
          }, 
        s => (Position) s));

    private static IEnumerable<KeyValuePair<TKey, TValue>> Pairs<TKey, TValue>(IEnumerable<TValue> list, Converter<TValue, TKey> firstConverter)
    {
      int i = 0;
      TValue first = default(TValue);
      foreach (var item in list)
      {
        if (++i % 2 == 1)
          first = item;
        else
          yield return new KeyValuePair<TKey, TValue>(firstConverter(first), item);
      }
    }
    private static Dictionary<TKey, TValue> Dic<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> source)
    {
      var result = new Dictionary<TKey, TValue>();
      foreach (var pair in source)
        result.Add(pair.Key, pair.Value);
      return result;
    }

    public static void InititBoard(Board board)
    {
      foreach (var pair in InitialPosition)
      {
        var party = pair.Key.Y < 5 ? board.White : board.Black;
        board[pair.Key] = new Piece(party, pair.Value);
      }
    }
  }
}