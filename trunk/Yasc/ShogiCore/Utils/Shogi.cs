using System;
using System.Collections.Generic;

namespace Yasc.ShogiCore.Utils
{
  public static class Shogi
  {
    public static readonly Dictionary<Position, string> InitialPosition = Dic(Pairs(new[]
                      {
                        "a1", "香", "a9", "香", "i1", "香", "i9", "香",
                        "a2", "桂", "a8", "桂", "i2", "桂", "i8", "桂",
                        "a3", "銀", "a7", "銀", "i3", "銀", "i7", "銀",
                        "a4", "金", "a6", "金", "i4", "金", "i6", "金",
                        "a5", "玉", "i5", "玉", "b2", "角", "h8", "角",
                        "b8", "飛", "h2", "飛", "c1", "歩", "c2", "歩",
                        "c3", "歩", "c4", "歩", "c5", "歩", "c6", "歩",
                        "c7", "歩", "c8", "歩", "c9", "歩", "g1", "歩",
                        "g2", "歩", "g3", "歩", "g4", "歩", "g5", "歩",
                        "g6", "歩", "g7", "歩", "g8", "歩", "g9", "歩",
                      }, 
                    s => (Position) s));

    private static IEnumerable<KeyValuePair<K, T>> Pairs<K, T>(IEnumerable<T> list, Converter<T, K> firstConverter)
    {
      int i = 0;
      T first = default(T);
      foreach (var item in list)
      {
        if (++i % 2 == 1)
          first = item;
        else
          yield return new KeyValuePair<K, T>(firstConverter(first), item);
      }
    }
    private static Dictionary<K, V> Dic<K, V>(IEnumerable<KeyValuePair<K, V>> source)
    {
      var result = new Dictionary<K, V>();
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