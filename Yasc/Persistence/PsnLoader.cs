using System;
using System.Collections.Generic;
using System.IO;
using Yasc.ShogiCore;
using System.Linq;
using Yasc.ShogiCore.Moves;

namespace Yasc.Persistence
{
  public class PsnLoader
  {
    private readonly Board _board;
    private readonly List<string> _moves = new List<string>();

    public PsnLoader()
    {
      _board = new Board();
      Shogi.InitBoard(_board);
      // TODO: Black start by default!
      _board.OneWhoMoves = _board.Black;
    }

    public Board Load(string content)
    {
      Load(new StringReader(content));
      return _board;
    }
    public void Load(TextReader content)
    {
      foreach (var line in content.Lines())
      {
        if (line.StartsWith("["))
        {
          ReadHeader(line);
        }
        else if (line.StartsWith("{"))
        {
          ReadFooter(line);
        }
        else
        {
          ReadBody(line);
        }
      }
    }

    private void ReadBody(string line)
    {
      var split = line.Split(new[] {' ', '\t'}, 
        StringSplitOptions.RemoveEmptyEntries);

      foreach (var move in split)
        ReadMove(move);
    }

    private void ReadMove(string move)
    {
      var split = move.Split('.');
      var number = int.Parse(split[0]);
      if (number != _moves.Count + 1) throw new Exception("Wrong move number");
      _moves.Add(split[1]);
    }

    private void ReadFooter(string line)
    {
      var movesB = new MovesB(_board, _moves);
      movesB.Start();
    }

    private void ReadHeader(string line)
    {
      if (line.StartsWith("[White "))
      {
        _board.White.Name = ReadAttribute("[White ", line);
      }
      else if (line.StartsWith("[Black "))
      {
        _board.Black.Name = ReadAttribute("[Black ", line);
      }
    }

    private static string ReadAttribute(string name, string line)
    {
      return line.Substring(name.Length  +1,
        line.Length - name.Length - "\"]".Length-1);
    }
  }

  internal class MovesB
  {
    private readonly Board _board;
    private readonly List<string> _moves;

    public MovesB(Board board, List<string> moves)
    {
      _board = board;
      _moves = moves;
    }

    public void Start()
    {
      if (!Start(0)) throw new Exception("Invalid moves?");
    }

    private bool Start(int index)
    {
      // We reached the end successfully
      if (index == _moves.Count) return true;

      // We've just one choice here
      MoveBase[] choice;
      while (true)
      {
        choice = _board.GetMove(_moves[index], MoveNotation.Cute).ToArray();
        if (choice.Length != 1) break;
        _board.MakeMove(choice[0]);
        if (++index == _moves.Count) return true;
      }
      
      // We've 0 or >1 choices
      foreach (var option in choice)
      {
        _board.MakeMove(option);
        bool success = Start(index + 1);
        // This one is good
        if (success) return true;
        // Hope we've next cycle with one more choice
        _board.History.CurrentMoveIndex = index - 1;
      }

      // No good choice
      return false;
    }
  }

  public static class TextReaderExtensions
  {
    public static IEnumerable<string> Lines(this TextReader reader)
    {
      if (reader == null) throw new ArgumentNullException("reader");
      return LinesInternal(reader);
    }

    private static IEnumerable<string> LinesInternal(TextReader reader)
    {
      while (true)
      {
        var line = reader.ReadLine();
        if (line == null) break;
        yield return line;
      }
    }
  }
}
