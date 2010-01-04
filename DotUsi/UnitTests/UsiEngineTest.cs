using System;
using DotUsi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
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

    [TestMethod]
    public void UsiEngineConstructorTest()
    {
      _engine.Usi();
      _engine.Dispose();
      Assert.AreEqual("usi", _process.InputData.Dequeue());
      Assert.AreEqual("isready", _process.InputData.Dequeue());
      Assert.AreEqual("quit", _process.InputData.Dequeue());
    }
    [TestMethod]
    public void TestId()
    {
      _engine.Usi();
      _process.SendOutput("id name 1");
      _process.SendOutput("id author 2");
      Assert.AreEqual("1", _engine.EngineName);
      Assert.AreEqual("2", _engine.AuthorName);
    }
    [TestMethod]
    public void TestMode()
    {
      Assert.AreEqual(EngineMode.Started, _engine.Mode);
      _engine.Usi();
      Assert.AreEqual(EngineMode.Processing, _engine.Mode);
      _process.SendOutput("readyok");
      Assert.AreEqual(EngineMode.Ready, _engine.Mode);
    }
    [TestMethod]
    public void TestInapropriateId()
    {
      _process.SendOutput("id name 1");
      _process.SendOutput("id author 2");
      Assert.IsNull(_engine.EngineName);
      Assert.IsNull(_engine.AuthorName);
    }
    [TestMethod, ExpectedException(typeof(UsiParserError))]
    public void OptionWithoutTypeTest()
    {
      _engine.Usi();
      _process.SendOutput("option name 1"); 
    }
    [TestMethod, ExpectedException(typeof(UsiParserError))]
    public void OptionWithoutNameTest()
    {
      _engine.Usi();
      _process.SendOutput("option type string"); 
    }
    [TestMethod]
    public void TestSimplestOption()
    {
      _engine.Usi();
      _process.SendOutput("option name 1 type string");
      Assert.AreEqual(1, _engine.Options.Count);
      Assert.AreEqual("1", _engine.Options[0].Name);
      Assert.AreEqual(UsiOptionType.String, _engine.Options[0].OptionType);
    }
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
      _engine.Usi();
      _process.InputData.Clear();
      _process.SendOutput("readyok");
      _engine.Position();
      Assert.AreEqual("position startpos", _process.InputData.Dequeue());
    }
    [TestMethod]
    public void TestStartPositionWithMoves()
    {
      _engine.Usi();
      _process.InputData.Clear();
      _process.SendOutput("readyok");
      _engine.Position("1g1i");
      Assert.AreEqual("position startpos moves 1g1i", _process.InputData.Dequeue());
    }
    [TestMethod]
    public void TestSfenPosition()
    {
      _engine.Usi();
      _process.InputData.Clear();
      _process.SendOutput("readyok");
      _engine.Position(new SfenString("1"));
      Assert.AreEqual("position sfen 1", _process.InputData.Dequeue());
    }
    [TestMethod]
    public void TestSfenPositionWithMoves()
    {
      _engine.Usi();
      _process.InputData.Clear();
      _process.SendOutput("readyok");
      _engine.Position(new SfenString("1"));
      Assert.AreEqual("position sfen 1", _process.InputData.Dequeue());
    }
    [TestMethod]
    public void TestDebugOnDifferentModes()
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
    [TestMethod]
    public void NewGame()
    {
      PrepareEngine();
      _engine.NewGame();
      Assert.AreEqual("usinewgame", _process.InputData.Dequeue());
      Assert.AreEqual("isready", _process.InputData.Dequeue());
      Assert.AreEqual(EngineMode.Processing, _engine.Mode);
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
    // I assume Go overloads don't require testing because of their simplicity
    [TestMethod]
    public void GoWithNullArgsTest()
    {
      PrepareEngine();
      _engine.Go(null, null, null);
      Assert.AreEqual("go infinite", _process.InputData.Dequeue());
      Assert.AreEqual(EngineMode.Searching, _engine.Mode);
    }
    [TestMethod]
    public void GoTest()
    {
      PrepareEngine();
      _engine.Go(new TimeConstraint { Byoyomi = TimeSpan.FromMilliseconds(1000) }, 
                 new DepthConstraint { Depth = 10 });

      Assert.AreEqual("go byoyomi 1000 depth 10", _process.InputData.Dequeue());
      Assert.AreEqual(EngineMode.Searching, _engine.Mode);
    }
    [TestMethod]
    public void GoSearchMovesTest()
    {
      PrepareEngine();
      _engine.Go(new TimeConstraint { Byoyomi = TimeSpan.FromMilliseconds(1000) }, 
                 new DepthConstraint { Depth = 10 });

      Assert.AreEqual("go byoyomi 1000 depth 10", _process.InputData.Dequeue());
      Assert.AreEqual(EngineMode.Searching, _engine.Mode);
    }

    private void PrepareEngine()
    {
      _engine.Usi();
      _process.SendOutput("readyok");
      _process.InputData.Clear();
    }
  }
}
