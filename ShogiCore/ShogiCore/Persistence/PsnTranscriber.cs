using System;
using System.Collections.Generic;
using System.IO;

namespace Yasc.ShogiCore.Persistence
{
  public class PsnTranscriber
  {
    private GameTrascription _currentTranscription;

    public IEnumerable<GameTrascription> Load(TextReader psn)
    {
      _currentTranscription = new GameTrascription();
      foreach (var line in psn.Lines())
      {
        if (line.Trim().Length == 0)
        {
          if (_currentTranscription.IsFull)
          {
            yield return _currentTranscription;
            _currentTranscription = new GameTrascription();
          }
        }
        else if (line.StartsWith("["))
        {
          _currentTranscription.AddProperty(ParseHeader(line));
        }
        else if (line.StartsWith("{"))
        {
          _currentTranscription.AddProperty(ParseFooter(line));
        }
        else 
        {
          ReadBody(line);
        }
      }
    }

    private static TrascriptionProperty ParseFooter(string line)
    {
      return new TrascriptionProperty
               {
                 Name = "_Footer", 
                 Value = line.TrimStart('{').TrimEnd('}')
               };
    }

    private static TrascriptionProperty ParseHeader(string line)
    {
      var res = new TrascriptionProperty();
      int qi = line.IndexOf('"');
      res.Name = line.Substring(1, qi - 2);
      res.Value = line.Substring(qi+1, line.Length - qi - 3);
      return res;
    }

    private void ReadBody(string line)
    {
      var split = line.Split(new[] { ' ', '\t' },
                             StringSplitOptions.RemoveEmptyEntries);

      foreach (var move in split)
        ReadMove(move);
    }

    private void ReadMove(string move)
    {
      var split = move.Split('.');
      var number = int.Parse(split[0]);
      if (number != _currentTranscription.Moves.Count + 1) 
        throw new Exception("Wrong move number");
      _currentTranscription.Moves.Add(split[1]);
    }
  }
}