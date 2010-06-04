using System.Collections.Generic;
using System.IO;
using Yasc.Utils;

namespace Yasc.ShogiCore.Persistence
{
  public class Ki2Transcriber
  {
    public IEnumerable<GameTranscription> Load(TextReader psn)
    {
      var curr = new GameTranscription();
      var hasBody = false;
      foreach (var line in psn.Lines())
      {
        if (line.Contains(":") && !line.StartsWith("*"))
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
      var header = line.Split(':');
      return new TrascriptionProperty
               {
                 Name = header[0],
                 Value = header[1]
               };
    }
  }
}