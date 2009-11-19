using System;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Moves;
using Yasc.ShogiCore.Utils;
using System.Linq;

namespace Yasc.AI
{
  public class RandomAiController : AiControllerBase
  {
    private readonly Board _board;
    private readonly static Random _rnd = new Random();

    public RandomAiController()
    {
      _board = new Board();
      Shogi.InititBoard(_board);
    }

    protected override void OnHumanMoved(string hisMove)
    {
      _board.MakeMove(_board.GetMove(hisMove));
      var myMove = ChooseAbsolutelyRandomMove();
      if (myMove == null) return; // mate
      _board.MakeMove(myMove);
      Move(myMove.ToString());
    }

    private MoveBase ChooseAbsolutelyRandomMove()
    {
      var moves = SituationAnalizer.GetAllValidMoves(
        _board.CurrentSnapshot, PieceColor.Black).ToList();

      if (moves.Count == 0) return null; // mate
      var m = moves[_rnd.Next(moves.Count)];
      return m.AsRealMove(_board);
    }
  }
}