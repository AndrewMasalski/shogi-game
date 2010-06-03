using System;
using System.Collections.Generic;
using System.Text;
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.Notations;
using Yasc.ShogiCore.Snapshots;
using System.Linq;

namespace Yasc.ShogiCore.Persistence
{
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
      ParseCommentsAndMoves(Body.ToString(), 
        new[] {'(', '{'},  new[] {')', '}'});
      FixResignMove();
      Body = null;
    }

    private void FixResignMove()
    {
      if (Moves.Count > 0)
      {
        var last = Moves[Moves.Count - 1];
        if (last.MoveNotation == null && last.Comment.ToLower() == "resign")
        {
          last.MoveNotation = "resign";
          last.Comment = null;
        }
      }
    }

    private void ParseCommentsAndMoves(string body, char[] open, char[] close)
    {
      var index = 0;
      while (index < body.Length)
      {
        var startIndex = body.IndexOfAny(open, index);
        if (startIndex == -1)
        {
          ReadMove(index == 0 ? body : body.Substring(index, body.Length - index));
          break;
        }
        if (startIndex - index > 0)
        {
          ReadMove(body.Substring(index, startIndex - index));
        }
        var openBracetType = Array.IndexOf(open, body[startIndex]);
        var endIndex = body.IndexOf(close[openBracetType], startIndex);
        ReadComment(body.Substring(startIndex+1, endIndex - startIndex-1));
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
        new[] { ' ', '\t', '.', '\r', '\n' }, 
        StringSplitOptions.RemoveEmptyEntries);

      foreach (var move in split)
        ReadOneMove(move);
      
    }
    private void ReadOneMove(string move)
    {
      int num;
      if (int.TryParse(move, out num))
      {
        Moves.Add(new MoveTranscription { Number = num});
      }
      else
      {
        if (Moves.Count > 0 && Moves[Moves.Count - 1].MoveNotation == null)
        {
          ParseNotation(Moves[Moves.Count - 1], move);
        }
        else
        {
          Moves.Add(ParseNotation(new MoveTranscription(), move));
        }
      }
    }

    private static MoveTranscription ParseNotation(MoveTranscription moveTranscription, string move)
    {
      if (move.EndsWith("!?") || move.EndsWith("??") || move.EndsWith("!!"))
      {
        moveTranscription.Evaluation = move.Substring(move.Length - 2, 2);
        moveTranscription.MoveNotation = move.Substring(0, move.Length - 2).TrimEnd();
      }
      else if (move.EndsWith("!") || move.EndsWith("?"))
      {
        moveTranscription.Evaluation = move.Substring(move.Length - 1, 1);
        moveTranscription.MoveNotation = move.Substring(0, move.Length - 1).TrimEnd();
      }
      else
      {
        moveTranscription.MoveNotation = move;
      }
      return moveTranscription;
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