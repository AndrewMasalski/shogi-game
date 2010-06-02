using System;
using System.Collections.Generic;
using System.Text;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.Notations;
using Yasc.ShogiCore.Snapshots;
using System.Linq;

namespace Yasc.ShogiCore.Persistence
{
  public class MoveTranscription
  {
    public string Number { get; set; }
    public string MoveNotation { get; set; }
    public string Comment { get; set; }
    public override string ToString()
    {
      var sb = new StringBuilder();
      if (Number != null)
      {
        sb.Append(Number);
        sb.Append(". ");
      }
      sb.Append(MoveNotation);
      if (!string.IsNullOrWhiteSpace(Comment))
      {
        sb.Append(" // ");
        sb.Append(Comment);
      }
      return sb.ToString();
    }
  }
  public class GameTranscription
  {
    public Dictionary<string, TrascriptionProperty> Properties { get; private set; }

    public List<MoveTranscription> Moves { get; private set; }

    public StringBuilder Body { get; private set; }

    public string GameComment
    {
      get; private set;
    }

    public GameTranscription()
    {
      Properties = new Dictionary<string, TrascriptionProperty>();
      Moves = new List<MoveTranscription>();
      Body = new StringBuilder();
    }

    public void ParseBody()
    {
      if (Body == null) return;

      var s = Body.ToString();

      A(s, new[] {'(', '{'},  new[] {')', '}'}, ReadMove, ReadComment);

      Body = null;
    }

    private void A(string s, char[] open, char[] close, Action<string> readMove, Action<string> readComment)
    {
      var index = 0;
      while (index < s.Length)
      {
        var startIndex = s.IndexOfAny(open, index);
        if (startIndex == -1)
        {
          readMove(index == 0 ? s : s.Substring(index, s.Length - index));
          break;
        }
        if (startIndex - index > 0)
        {
          readMove(s.Substring(index, startIndex - index ));
        }
        var openBracetType = Array.IndexOf(open, s[startIndex]);
        var endIndex = s.IndexOf(close[openBracetType], startIndex);
        readComment(s.Substring(startIndex+1, endIndex - startIndex-1));
        index = endIndex + 1;
      }
    }

    private void ReadComment(string comment)
    {
      if (Moves.Count == 0)
        GameComment = comment;
      else
        Moves.Last().Comment = comment;
    }

    private void ReadMove(string moveSeq)
    {
      var split = moveSeq.Split(
        new[] { ' ', '\t' }, 
        StringSplitOptions.RemoveEmptyEntries);

      foreach (var move in split)
        ReadOneMove(move);
      
    }
    private void ReadOneMove(string move)
    {
      var strings = move.Split('.');
      var res = new MoveTranscription();
      if (strings.Length == 2)
      {
        res.Number = strings[0];
      }
      res.MoveNotation = strings.Last();
      Moves.Add(res);
    }

    public void AddProperty(TrascriptionProperty property)
    {
      Properties[property.Name] = property;
    }

    public BoardSnapshot LoadSnapshot()
    {
      ParseBody();
      var loader = new AmbiguousMoveSequencesLoader(
        BoardSnapshot.InitialPosition,
        Moves.Select(m => m.MoveNotation).ToList(),
        CuteNotation.Instance);
      return loader.Load();
    }
    public Board LoadBoard(IPieceSet pieceSet)
    {
      ParseBody();
      var board = new Board(pieceSet);
      board.LoadSnapshotWithHistory(LoadSnapshot());
      board.White.Name = Properties["White"].Value;
      board.Black.Name = Properties["Black"].Value;
      return board;

    }
  }
}