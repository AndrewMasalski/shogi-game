using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using MainModule.Properties;
using Yasc.DotUsi;
using Yasc.DotUsi.Drivers;
using Yasc.DotUsi.Options;
using Yasc.DotUsi.Process;
using Yasc.ShogiCore;
using Yasc.ShogiCore.Moves;

namespace MainModule.AI
{
  public class UsiAiController : AiControllerBase, IDisposable
  {
    private readonly Board _board;
    private readonly UsiEngine _engine;

    private UsiAiController(IUsiProcess engine)
    {
      _board = new Board();
      Shogi.InitBoard(_board);
      _engine = CreateEngine(engine);
    }

    public static UsiAiController Create()
    {
      var engineProcess = CreateEngineProcess();
      return engineProcess == null ? null : 
        new UsiAiController(engineProcess);
    }

    private static IUsiProcess CreateEngineProcess()
    {
      var enginePath = Path.Combine(
        Environment.CurrentDirectory,
        Settings.Default.CurrentEngine);

      if (!File.Exists(enginePath))
      {
        MessageBox.Show("Couldn't find engine file: " + enginePath, 
          "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        return null;
      }
      try
      {
        return new SpearCsa2008V14Driver(
          new UsiWindowsProcess(enginePath));
      }
      catch (Win32Exception x)
      {
        MessageBox.Show("An error occured while starting engine: " + x.Message,
          "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        return null;
      }
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
        _board.History.Select(MoveToUsiString)));
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