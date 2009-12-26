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

    public PsnLoader()
    {
      _board = new Board();
      Shogi.InitBoard(_board);
      // TODO: Black start by default!
      _board.OneWhoMoves = _board.Black;
    }

    public Board Load(GameTrascription trascription)
    {
      var movesB = new MovesB(_board, trascription.Moves);
      movesB.Start();
      _board.White.Name = trascription.Attributes["White"].Value;
      _board.Black.Name = trascription.Attributes["Black"].Value;
      return _board;
    }

  }

  /// <summary>Loads given <see cref="MoveNotation.Cute"/> move sequence to the 
  ///   <see cref="Board"/>.<see cref="Board.History"/> resolving ambiguities 
  ///   from the conext.</summary>
  /// <remarks><see cref="MoveNotation.Cute"/> moves are not completely definitive
  ///   so we might need to solve ambiguities from conext. To be more precise
  ///   after ambiguity found, we go on for all variants of transcription until
  ///   some furhter moves don't cross invalid options out.</remarks>
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

  public class GameTrascription
  {
    public Dictionary<string, TrascriptionAttibute> Attributes { get; private set; }

    public bool IsFull
    {
      get { return Attributes.Count > 0 && Moves.Count > 0; }
    }

    public List<string> Moves { get; private set; }

    public GameTrascription()
    {
      Attributes = new Dictionary<string, TrascriptionAttibute>();
      Moves = new List<string>();
    }

    public void AddAttribute(TrascriptionAttibute attibute)
    {
      Attributes[attibute.Name] = attibute;
    }
  }
  public class TrascriptionAttibute
  {
    public string Name { get; set; }
    public string Value { get; set; }
  }
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
          _currentTranscription.AddAttribute(ParseHeader(line));
        }
        else if (line.StartsWith("{"))
        {
          _currentTranscription.AddAttribute(ParseFooter(line));
        }
        else 
        {
          ReadBody(line);
        }
      }
    }

    private static TrascriptionAttibute ParseFooter(string line)
    {
      return new TrascriptionAttibute
               {
                 Name = "_Footer", 
                 Value = line.TrimStart('{').TrimEnd('}')
               };
    }

    private static TrascriptionAttibute ParseHeader(string line)
    {
      var res = new TrascriptionAttibute();
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
