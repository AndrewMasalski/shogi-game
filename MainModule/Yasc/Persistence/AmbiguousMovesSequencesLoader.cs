using System;
using System.Collections.Generic;
using System.Linq;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Moves;

namespace Yasc.Persistence
{
  /// <summary>Loads given <see cref="MoveNotation.Cute"/> move sequence to the 
  ///   <see cref="Board"/>.<see cref="Board.History"/> resolving ambiguities 
  ///   from the conext.</summary>
  /// <remarks><see cref="MoveNotation.Cute"/> moves are not completely definitive
  ///   so we might need to solve ambiguities from conext. To be more precise
  ///   after ambiguity found, we go on for all variants of transcription until
  ///   some furhter moves don't cross invalid options out.</remarks>
  internal class AmbiguousMovesSequencesLoader
  {
    private readonly Board _board;
    private readonly List<string> _moves;

    public AmbiguousMovesSequencesLoader(Board board, List<string> moves)
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
}