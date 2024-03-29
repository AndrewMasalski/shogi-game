using System;
using System.Collections.Generic;
using System.IO;
using Yasc.Utils;

namespace Yasc.ShogiCore.Persistence
{
  public class PsnTranscriber
  {
    public IEnumerable<GameTranscription> Load(TextReader psn)
    {
      var curr = new GameTranscription();
      var hasBody = false;
      foreach (var line in psn.Lines())
      {
        if (line.StartsWith("["))
        {
          if (curr.Properties.Count > 0 && hasBody)
          {
            yield return curr;
            curr = new GameTranscription();
            hasBody = false;
          }

          curr.AddProperty(ParseHeader(line));
        }
        else if (line.Trim().Length != 0)
        {
          curr.Body.AppendLine(line);
          hasBody = true;
        }
      }
      if (curr.Properties.Count > 0 && hasBody)
        yield return curr;
    }

    private static TrascriptionProperty ParseHeader(string line)
    {
      var res = new TrascriptionProperty();
      int qi = line.IndexOf('"');
      res.Name = line.Substring(1, qi - 2);
      res.Value = line.Substring(qi + 1, line.Length - qi - 3);
      return res;
    }
  }
}