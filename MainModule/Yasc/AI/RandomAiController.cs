using System;
using System.Threading;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Moves;
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
      Shogi.InitBoard(_board);
    }

    public override void UndoLastMove()
    {
      if (_board.History.CurrentMove.Who.Color != MyColor)
        throw new Exception("Wait until comp stop thinking!");
      
      _board.History.CurrentMoveIndex -= 2;
    }

    protected override void OnHumanMoved(string hisMove)
    {
      Thread.Sleep(1000);
      _board.MakeMove(_board.GetMove(hisMove));
      var myMove = ChooseAbsolutelyRandomMove();
      if (myMove == null) return; // mate
      _board.MakeMove(myMove);
      Move(myMove.ToString());
    }

    private MoveBase ChooseAbsolutelyRandomMove()
    {
      var moves = _board.CurrentSnapshot.
        GetAllAvailableMoves(PieceColor.Black).ToList();

      if (moves.Count == 0) return null; // mate
      var m = moves[_rnd.Next(moves.Count)];
      return m.AsRealMove(_board);
    }
  }
}