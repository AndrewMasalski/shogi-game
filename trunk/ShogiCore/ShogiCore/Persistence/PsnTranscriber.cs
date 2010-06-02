using System;
using System.Collections.Generic;
using System.IO;
using Yasc.Utils;

namespace Yasc.ShogiCore.Persistence
{
  public class PsnTranscriber
  {
    private GameTranscription _currentTranscription;

    public IEnumerable<GameTranscription> Load(TextReader psn)
    {
      _currentTranscription = new GameTranscription();
      var hasBody = false;
      foreach (var line in psn.Lines())
      {
        if (line.StartsWith("["))
        {
          if (_currentTranscription.Properties.Count > 0 && hasBody)
          {
            yield return _currentTranscription;
            _currentTranscription = new GameTranscription();
            hasBody = false;
          }

          _currentTranscription.AddProperty(ParseHeader(line));
        }
        else if (line.Trim().Length != 0)
        {
          _currentTranscription.Body.Append(line);
          hasBody = true;
        }
      }
      if (_currentTranscription.Properties.Count > 0 && hasBody)
        yield return _currentTranscription;
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