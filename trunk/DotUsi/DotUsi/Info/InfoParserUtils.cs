using System;
using System.Collections.Generic;
using System.Linq;
using DotUsi.Utils;

namespace DotUsi.Info
{
  internal static class InfoParserUtils
  {
    public static void ParseLine(Dictionary<string, Action<string>> parserTable, string line)
    {
      var split = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
      for (int i = 0; i < split.Length; i++)
      {
        Action<string> action;
        if (parserTable.TryGetValue(split[i], out action))
        {
          int start = ++i;
          while (i + 1 < split.Length && !parserTable.ContainsKey(split[i + 1]))
            i++;
          string[] strings = split.Range(start, i - start + 1).ToArray();
          action(string.Join(" ", strings));
        }
      }
    }    
  }
}