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
using Yasc.ShogiCore.Core;
using Yasc.ShogiCore.Notations;
using Yasc.ShogiCore.PieceSets;
using Yasc.ShogiCore.Snapshots;

namespace MainModule.AI
{
  public class UsiAiController : AiControllerBase, IDisposable
  {
    private readonly Board _board;
    private readonly UsiEngine _engine;

    private UsiAiController(IUsiProcess engine)
    {
      _board = new Board(new StandardPieceSet());
      _board.LoadSnapshot(BoardSnapshot.InitialPosition);
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
        _board.MakeWrapedMove(engineMove);
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
      _board.MakeWrapedMove(_board.GetMove(hisMove, FormalNotation.Instance).First());
      _engine.Position(string.Join(" ", 
        _board.History.Select(m => MoveToUsiString(m.Move))));
      _engine.Go();
    }

    /// <summary>
    /// TODO: Create a native implementation
    /// </summary>
    private static string MoveToUsiString(MoveSnapshotBase move)
    {
      var usualMove = move as UsualMoveSnapshot;
      if (usualMove != null)
      {
        return usualMove.From + usualMove.To.ToString() +
          (usualMove.IsPromoting ? "+" : "");
      }
      // BUG: Resign move!?
      var dropMove = (DropMoveSnapshot) move;
      return dropMove.PieceType.Latin + "*" + dropMove.To;
    }

    /// <summary>
    /// TODO: Create a native implementation
    /// </summary>
    private static MoveSnapshotBase ParseUsiMove(Board board, string usiMove)
    {
      return usiMove.Contains("*") ?
        board.GetMove(usiMove.Replace('*', '\''), FormalNotation.Instance).First() :
        board.GetMove(usiMove.Insert(2, "-"), FormalNotation.Instance).First();
    }

    public void Dispose()
    {
      _engine.Dispose();
    }
  }
}