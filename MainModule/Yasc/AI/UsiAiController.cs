using System;
using System.IO;
using System.Linq;
using DotUsi;
using Yasc.Properties;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Moves;

namespace Yasc.AI
{
  public class UsiAiController : AiControllerBase, IDisposable
  {
    private readonly Board _board;
    private readonly UsiEngine _engine;

    public UsiAiController()
    {
      _board = new Board();
      Shogi.InitBoard(_board);
      _engine = CreateEngine(CreateEngineProcess());
    }

    private static IUsiProcess CreateEngineProcess()
    {
      var enginePath = Path.Combine(
        Environment.CurrentDirectory,
        Settings.Default.CurrentEngine);

      return new SpearCsa2008V14Driver(
        new UsiWindowsProcess(enginePath));
    }

    private UsiEngine CreateEngine(IUsiProcess process)
    {
      var engine = new UsiEngine(process);
      engine.SynchUsi();
      engine.SetImplicitOptions();
      engine.Options.Where(o => o.Name == "SpearLevel").Cast<SpinOption>().First().Value = Settings.Default.EngineLevel;
      engine.SynchIsReady();
      engine.SynchNewGame();
      engine.BestMove += EngineOnBestMove;
      return engine;
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

    public override void UndoLastMove()
    {
//      if (_board.History.CurrentMove.Who.Color != MyColor)
//        throw new Exception("Wait until comp stop thinking!");
      
      _board.History.CurrentMoveIndex -= 2;
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

    public void Dispose()
    {
      _engine.Dispose();
    }
  }
}