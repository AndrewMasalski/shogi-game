using System;
using System.IO;
using System.Linq;
using DotUsi;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Moves;

namespace Yasc.AI
{
  public class UsiAiController : AiControllerBase
  {
    private readonly Board _board;
    private readonly UsiEngine _engine;

    public UsiAiController()
    {
      _board = new Board();
      Shogi.InitBoard(_board);

      var enginePath = Path.Combine(
        Environment.CurrentDirectory,
        @"Engines\Spear\SpearShogidokoro.exe");

      var process =
          new SpearCsa2009V15Driver(
          new UsiWindowsProcess(enginePath));

      _engine = new UsiEngine(process);
      _engine.SynchUsi();
      _engine.Options.Where(o => o.Name == "SpearLevel").Cast<SpinOption>().First().Value = 3;
      _engine.SynchIsReady();
      _engine.SynchNewGame();
      _engine.BestMove += EngineOnBestMove;
    }

    private void EngineOnBestMove(object sender, BestMoveEventArgs args)
    {
      if (args.Resign)
      {
        Move("resign");
      }
      else
      {
        var engineMove = ParseUsiMove(_board, args.Move);
        _board.MakeMove(engineMove);
        Move(engineMove.ToString());
      }
    }

    protected override void OnHumanMoved(string hisMove)
    {
      _board.MakeMove(_board.GetMove(hisMove));
      _engine.Position(string.Join(" ",
        _board.History.Select(move => MoveToUsiString(move)).ToArray()));
      _engine.Go();
    }

    /// <summary>
    /// TODO: Create a native implementation
    /// </summary>
    private static string MoveToUsiString(MoveBase move)
    {
      var usualMove = move as UsualMove;
      if (usualMove != null)
      {
        return usualMove.From + usualMove.To.ToString() +
          (usualMove.IsPromoting ? "+" : "");
      }
      var dropMove = (DropMove) move;
      return dropMove.PieceType.Latin + "*" + dropMove.To;
    }

    /// <summary>
    /// TODO: Create a native implementation
    /// </summary>
    private static MoveBase ParseUsiMove(Board board, string usiMove)
    {
      return usiMove.Contains("*") ?
        board.GetMove(usiMove.Replace('*', '\'')) :
        board.GetMove(usiMove.Insert(2, "-"));
    }
  }
}