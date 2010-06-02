using System;
using System.Collections.Generic;
using System.Linq;
using Yasc.ShogiCore.Notations;
using Yasc.ShogiCore.Snapshots;

namespace Yasc.ShogiCore.Persistence
{
  /// <summary>Loads given <see cref="CuteNotation"/> move sequence as a
  ///   <see cref="BoardSnapshot"/> resolving ambiguities from the conext.</summary>
  /// <remarks><see cref="CuteNotation"/> moves are not completely definitive
  ///   so we might need to solve ambiguities from conext. To be more precise
  ///   after ambiguity found, we go on for all variants of transcription until
  ///   some furhter moves don't cross invalid options out.</remarks>
  internal class AmbiguousMoveSequencesLoader
  {
    private BoardSnapshot _board;
    private readonly List<string> _moves;
    private readonly INotation _notation;

    public AmbiguousMoveSequencesLoader(
      BoardSnapshot initialPosition, List<string> moves, INotation notation)
    {
      _board = initialPosition;
      _moves = moves;
      _notation = notation;
    }

    public BoardSnapshot Load()
    {
      if (!Start(0)) throw new Exception("Invalid moves?");
      return _board;
    }

    private bool Start(int index)
    {
      // We reached the end successfully
      if (index == _moves.Count) return true;

      // We've just one choice here
      Move[] choice;
      while (true)
      {
        choice = _notation.Parse(_board, _moves[index]).ToArray();
        if (choice.Length != 1) break;
        _board = _board.MakeMove(choice[0]);
        if (++index == _moves.Count) return true;
      }

      var prev = _board;
      // We've 0 or >1 choices
      foreach (var option in choice)
      {
        _board = _board.MakeMove(option);
        bool success = Start(index + 1);
        // This one is good
        if (success) return true;
        // Hope we've next cycle with one more choice
        _board = prev;
      }

      // No good choice
      return false;
    }
  }
}