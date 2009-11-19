using System;
using System.Linq;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Moves;
using Yasc.ShogiCore.Utils;

namespace Yasc.AI
{
  public class CleverAiController : AiControllerBase
  {
    private readonly static Random _rnd = new Random();
    private readonly Board _board;

    public CleverAiController()
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

      var takes = from n in moves let u = n as UsualMoveSnapshot where u != null
                  let t = _board[u.To] where t != null select new { Move = u, t.Type};
      var best = takes.OrderByDescending(u => Cost(u.Type)).FirstOrDefault();
      
      if (best != null) return best.Move.AsRealMove(_board);

      var m = moves[_rnd.Next(moves.Count)];
      return m.AsRealMove(_board);
    }

    public int Cost(PieceType p)
    {
      switch ((string)p)
      {
        case "玉": return 100;
        case "飛": return 15;
        case "角": return 13;
        case "金": return 9;
        case "銀": return 8;
        case "桂": return 6;
        case "香": return 5;
        case "歩": return 1;
        case "竜": return 15;
        case "馬": return 17;
        case "全": return 9;
        case "今": return 10;
        case "仝": return 10;
        case "と": return 12;
      }
      throw new Exception();
      
    }
  }
}