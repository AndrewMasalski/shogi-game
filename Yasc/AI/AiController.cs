using System;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Moves;
using Yasc.ShogiCore.Utils;
using System.Linq;

namespace Yasc.AI
{
  public class AiController : AiControllerBase
  {
    private readonly Board _board;
    private readonly static Random _rnd = new Random();

    public AiController()
    {
      _board = new Board();
      Shogi.InititBoard(_board);
    }

    protected override void OnHumanMoved(string hisMove)
    {
      _board.MakeMove(_board.GetMove(hisMove));
      var myMove = ChooseAbsolutelyRandomMove();
      _board.MakeMove(myMove);
      Move(myMove.ToString());
    }

    private MoveBase ChooseAbsolutelyRandomMove()
    {
      var moves = SituationAnalizer.GetAllValidMoves(
        _board.CurrentSnapshot, PieceColor.Black).ToList();

      var m = moves[_rnd.Next(moves.Count)];
      return m.AsRealMove(_board);
    }
  }
}