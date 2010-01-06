using System;
using DotUsi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
  /// <remarks>
  /// <para>I assume Go overloads don't require testing because of their simplicity </para>
  /// <para>All modifiers, constraints, etc. must be tested further</para>
  /// </remarks>
  [TestClass]
  public class UsiEngineTest
  {
    private TestProcess _process;
    private UsiEngine _engine;

    [TestInitialize]
    public void Init()
    {
      _process = new TestProcess();
      _engine = new UsiEngine(_process);
    }

    #region ' Usi method and ID response '

    [TestMethod]
    public void UsiMethodTest()
    {
      _engine.Usi();
      _engine.Dispose();
      Assert.AreEqual("usi", _process.InputData.Dequeue());
      //      Assert.AreEqual("isready", _process.InputData.Dequeue());
      Assert.AreEqual("quit", _process.InputData.Dequeue());
    }
    [TestMethod]
    public void UsiResponseTest()
    {
      _engine.Usi();
      _process.SendOutput("id name 1");
      _process.SendOutput("id author 2");
      Assert.AreEqual("1", _engine.EngineName);
      Assert.AreEqual("2", _engine.AuthorName);
    }
    [TestMethod]
    public void UsiModeTest()
    {
      Assert.AreEqual(EngineMode.Started, _engine.Mode);
      _engine.Usi();
      Assert.AreEqual(EngineMode.Usi, _engine.Mode);
      _process.SendOutput("usiok");
      Assert.AreEqual(EngineMode.Ready, _engine.Mode);
    }
    [TestMethod]
    public void UsiInapropriateResponseTest()
    {
      _process.SendOutput("id name 1");
      _process.SendOutput("id author 2");
      Assert.IsNull(_engine.EngineName);
      Assert.IsNull(_engine.AuthorName);
    }

    #endregion

    #region ' Parse Options '

    [TestMethod, ExpectedException(typeof(UsiParserException))]
    public void ParseOptionWithoutTypeTest()
    {
      _engine.Usi();
      _process.SendOutput("option name 1");
    }
    [TestMethod, ExpectedException(typeof(UsiParserException))]
    public void ParseOptionWithoutNameTest()
    {
      _engine.Usi();
      _process.SendOutput("option type string");
    }
    [TestMethod]
    public void ParseSimplestValidOptionTest()
    {
      _engine.Usi();
      _process.SendOutput("option name 1 type string");
      Assert.AreEqual(1, _engine.Options.Count);
      Assert.AreEqual("1", _engine.Options[0].Name);
      Assert.AreEqual(UsiOptionType.String, _engine.Options[0].OptionType);
    }

    #endregion

    #region ' Position + 1 method '

    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void CallStartPositionInStartMode()
    {
      _engine.Position();
    }
    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void CallSfenPositionInStartMode()
    {
      _engine.Position(new SfenString("1"));
    }
    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void CallStartPositionInProcessingMode()
    {
      _engine.Usi();
      _engine.Position();
    }
    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void CallSfenPositionInProcessingMode()
    {
      _engine.Usi();
      _engine.Position(new SfenString("1"));
    }
    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void CallStartPositionInSearchMode()
    {
      PrepareEngine();
      _engine.Go();
      _engine.Position();
    }
    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void CallSfenPositionInSearchMode()
    {
      PrepareEngine();
      _engine.Go();
      _engine.Position(new SfenString("1"));
    }
    [TestMethod]
    public void TestStartPosition()
    {
      PrepareEngine();

      _engine.Position();
      Assert.AreEqual("position startpos", _process.InputData.Dequeue());
    }
    [TestMethod]
    public void TestStartPositionWithMoves()
    {
      PrepareEngine();
      _engine.Position("1g1i");
      Assert.AreEqual("position startpos moves 1g1i", _process.InputData.Dequeue());
    }
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void TestSfenPositionWithNullSfen()
    {
      _engine.Usi();
      _process.SendOutput("readyok");
      _engine.Position((SfenString)null);
    }
    [TestMethod]
    public void TestSfenPosition()
    {
      PrepareEngine();
      _engine.Position(new SfenString("1"));
      Assert.AreEqual("position sfen 1", _process.InputData.Dequeue());
    }
    [TestMethod]
    public void TestSfenPositionWithMoves()
    {
      PrepareEngine();
      _engine.Position(new SfenString("1"), "1i1a", "1a1i");
      Assert.AreEqual("position sfen 1 moves 1i1a 1a1i", _process.InputData.Dequeue());
    }

    #endregion

    #region ' Debug property '

    [TestMethod]
    public void TestDebugPropertyInDifferentModes()
    {
      Assert.IsFalse(_engine.DebugMode);
      _engine.DebugMode = true;
      Assert.AreEqual("debug on", _process.InputData.Dequeue());
      _engine.Usi();
      _process.InputData.Clear();
      _engine.DebugMode = true;
      Assert.AreEqual(0, _process.InputData.Count);
      _engine.DebugMode = false;
      Assert.AreEqual("debug off", _process.InputData.Dequeue());
      _process.SendOutput("readyok");
      _engine.DebugMode = true;
      Assert.AreEqual("debug on", _process.InputData.Dequeue());
    }

    #endregion

    #region ' NewGame method '

    [TestMethod]
    public void NewGame()
    {
      PrepareEngine();
      _engine.NewGame();
      Assert.AreEqual("usinewgame", _process.InputData.Dequeue());
      Assert.AreEqual(EngineMode.Corrupted, _engine.Mode);
      _engine.IsReady();
      Assert.AreEqual("isready", _process.InputData.Dequeue());
      _process.SendOutput("readyok");
      Assert.AreEqual(EngineMode.Ready, _engine.Mode);
    }
    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void NewGameInStartMode()
    {
      _engine.NewGame();
    }
    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void NewGameInProcessingMode()
    {
      _engine.Usi();
      _engine.NewGame();
    }
    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void NewGameInSearchMode()
    {
      PrepareEngine();
      _engine.Go();
      _engine.NewGame();
    }

    #endregion

    #region ' Go + 7 method '

    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void GoInStartMode()
    {
      _engine.Go();
    }
    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void GoInProcessingMode()
    {
      _engine.Usi();
      _engine.Go();
    }
    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void GoInSearchMode()
    {
      PrepareEngine();
      _engine.NewGame();
      _engine.Go();
      _engine.Go();
    }
    [TestMethod]
    public void GoWithNoArgsTest()
    {
      PrepareEngine();
      _engine.Go();
      Assert.AreEqual("go", _process.InputData.Dequeue());
      Assert.AreEqual(EngineMode.Searching, _engine.Mode);
    }
    [TestMethod]
    public void GoWithAllArgsTest()
    {
      PrepareEngine();
      _engine.Go(new SearchMateModifier(4),
                 new ByoyomiModifier(TimeSpan.FromSeconds(15)),
                 new DepthConstraint(10));

      Assert.AreEqual("go mate 4 byoyomi 15000 depth 10", _process.InputData.Dequeue());
      Assert.AreEqual(EngineMode.Searching, _engine.Mode);
    }

    #endregion

    #region ' Dispose method '

    [TestMethod]
    public void CallDisposeInStartedModeTest()
    {
      Assert.AreEqual(EngineMode.Started, _engine.Mode);
      _engine.Dispose();
      Assert.AreEqual(EngineMode.Disposed, _engine.Mode);
      Assert.AreEqual("quit", _process.InputData.Dequeue());
      Assert.AreEqual("<TestProcess: Dispose()>", _process.InputData.Dequeue());
    }

    [TestMethod]
    public void CallDisposeInProcessingModeTest()
    {
      _engine.Usi();
      _process.InputData.Clear();
      Assert.AreEqual(EngineMode.Usi, _engine.Mode);
      _engine.Dispose();
      Assert.AreEqual(EngineMode.Disposed, _engine.Mode);
      Assert.AreEqual("quit", _process.InputData.Dequeue());
      Assert.AreEqual("<TestProcess: Dispose()>", _process.InputData.Dequeue());
    }

    [TestMethod]
    public void CallDisposeInReadyModeTest()
    {
      PrepareEngine();
      Assert.AreEqual(EngineMode.Ready, _engine.Mode);
      _engine.Dispose();
      Assert.AreEqual(EngineMode.Disposed, _engine.Mode);
      Assert.AreEqual("quit", _process.InputData.Dequeue());
      Assert.AreEqual("<TestProcess: Dispose()>", _process.InputData.Dequeue());
    }

    [TestMethod]
    public void CallDisposeInSearchingModeTest()
    {
      PrepareEngine();
      _engine.Go();
      _process.InputData.Clear();
      Assert.AreEqual(EngineMode.Searching, _engine.Mode);
      _engine.Dispose();
      Assert.AreEqual(EngineMode.Disposed, _engine.Mode);
      Assert.AreEqual("quit", _process.InputData.Dequeue());
      Assert.AreEqual("<TestProcess: Dispose()>", _process.InputData.Dequeue());
    }

    [TestMethod]
    public void CallDisposeInPonderingModeTest()
    {
      PrepareEngine();
      _engine.Go(new PonderModifier());
      _process.InputData.Clear();
      Assert.AreEqual(EngineMode.Pondering, _engine.Mode);
      _engine.Dispose();
      Assert.AreEqual(EngineMode.Disposed, _engine.Mode);
      Assert.AreEqual("quit", _process.InputData.Dequeue());
      Assert.AreEqual("<TestProcess: Dispose()>", _process.InputData.Dequeue());
    }

    [TestMethod]
    public void CallDisposeInDisposedModeTest()
    {
      _engine.Dispose();
      _process.InputData.Clear();
      Assert.AreEqual(EngineMode.Disposed, _engine.Mode);
      _engine.Dispose();
      Assert.AreEqual(EngineMode.Disposed, _engine.Mode);
      Assert.AreEqual(0, _process.InputData.Count);
    }

    #endregion

    #region ' Stop method '

    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void CallStopInStartedModeTest()
    {
      Assert.AreEqual(EngineMode.Started, _engine.Mode);
      _engine.Stop();
    }

    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void CallStopInProcessingModeTest()
    {
      _engine.Usi();
      Assert.AreEqual(EngineMode.Usi, _engine.Mode);
      _engine.Stop();
    }

    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void CallStopInReadyModeTest()
    {
      PrepareEngine();
      Assert.AreEqual(EngineMode.Ready, _engine.Mode);
      _engine.Stop();
    }

    [TestMethod]
    public void CallStopInSearchingModeTest()
    {
      PrepareEngine();
      _engine.Go();
      _process.InputData.Clear();
      Assert.AreEqual(EngineMode.Searching, _engine.Mode);
      _engine.Stop();
      Assert.AreEqual(EngineMode.Ready, _engine.Mode);
      Assert.AreEqual("stop", _process.InputData.Dequeue());
    }

    [TestMethod]
    public void CallStopInPonderingModeTest()
    {
      PrepareEngine();
      _engine.Go(new PonderModifier());
      _process.InputData.Clear();
      Assert.AreEqual(EngineMode.Pondering, _engine.Mode);
      _engine.Stop();
      Assert.AreEqual(EngineMode.Ready, _engine.Mode);
      Assert.AreEqual("stop", _process.InputData.Dequeue());
    }

    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void CallStopInDisposedModeTest()
    {
      _engine.Dispose();
      Assert.AreEqual(EngineMode.Disposed, _engine.Mode);
      _engine.Stop();
    }

    #endregion

    #region ' PonderHit method '

    #endregion

    private void PrepareEngine()
    {
      _engine.Usi();
      _process.SendOutput("usiok");
      _process.InputData.Clear();
    }
  }
}
