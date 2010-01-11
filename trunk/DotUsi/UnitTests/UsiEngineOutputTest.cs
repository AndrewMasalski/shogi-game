using System;
using System.Collections.Specialized;
using DotUsi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
  [TestClass]
  public class UsiEngineOutputTest
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
    public void TestUsiOkEvent()
    {
      var log = new TestLog();
      _engine.Usi();
      _engine.UsiOK += (s, e) => log.Write("usiok");
      _process.SendOutput("usiok");
      Assert.AreEqual("usiok", log.ToString());
    }
    [TestMethod]
    public void TestReadyOkEvent()
    {
      var log = new TestLog();
      PrepareEngine();
      _engine.NewGame();
      _engine.IsReady();
      _engine.ReadyOK += (s, e) => log.Write("readyok");
      _process.SendOutput("readyok");
      Assert.AreEqual("readyok", log.ToString());
    }
    [TestMethod]
    public void TestBestMove()
    {
      var log = new TestLog();
      PrepareEngine();
      _engine.Go();
      _engine.BestMove += (s, e) => log.Write("bestmove " + e.Move + " ponder " + e.Ponder);
      _process.SendOutput("bestmove m1 ponder m2");
      Assert.AreEqual("bestmove m1 ponder m2", log.ToString());
      Assert.AreEqual(EngineMode.Ready, _engine.Mode);
    }
    [TestMethod]
    public void TestBestMoveResign()
    {
      var log = new TestLog();
      PrepareEngine();
      _engine.Go();
      _engine.BestMove += (s, e) => log.Write("bestmove " + 
        e.Move + " ponder " + e.Ponder + " resign: " + e.Resign);

      _process.SendOutput("bestmove resign");
      Assert.AreEqual("bestmove  ponder  resign: True", log.ToString());
    }
    [TestMethod]
    public void TestInfoUpdated()
    {
      PrepareEngine();
      var log = new TestLog();
      _engine.Go();
      _engine.Info.PropertyChanged += (s, e) => log.Write(e.PropertyName);
      _process.SendOutput("info depth 100");
      Assert.AreEqual(100, _engine.Info.Depth);
      Assert.AreEqual("Depth", log.ToString());
    }
    [TestMethod]
    public void TestTwoInfoUpdated()
    {
      PrepareEngine();
      var log = new TestLog();
      _engine.Go();
      _engine.Info.PropertyChanged += (s, e) => log.Write(e.PropertyName);
      _process.SendOutput("info time 100 seldepth 300");
      Assert.AreEqual(TimeSpan.FromSeconds(.1), _engine.Info.Time);
      Assert.AreEqual(300, _engine.Info.SelectiveDepth);
      Assert.AreEqual("Time SelectiveDepth", log.ToString());
    }
    [TestMethod]
    public void TestNodesAndPv()
    {
      PrepareEngine();
      var log = new TestLog();
      _engine.Go();
      _engine.Info.PropertyChanged += (s, e) => log.Write(e.PropertyName);
      ((INotifyCollectionChanged) _engine.Info.PrincipalVariation).
        CollectionChanged += (s, e) => log.Write(e.Action.ToString());
      _process.SendOutput("info nodes 100 pv m1 m2 m3");
      CollectionAssert.AreEqual(new[]{"m1", "m2", "m3"}, _engine.Info.PrincipalVariation);
      Assert.AreEqual(100, _engine.Info.Nodes);
      Assert.AreEqual("Nodes Reset Add Add Add", log.ToString());
    }
    [TestMethod]
    public void TestPvAndNodes()
    {
      PrepareEngine();
      var log = new TestLog();
      _engine.Go();
      _engine.Info.PropertyChanged += (s, e) => log.Write(e.PropertyName);
      ((INotifyCollectionChanged) _engine.Info.PrincipalVariation).
        CollectionChanged += (s, e) => log.Write(e.Action.ToString());
      _process.SendOutput("info pv m1 m2 m3 nodes 100");
      CollectionAssert.AreEqual(new[]{"m1", "m2", "m3"}, _engine.Info.PrincipalVariation);
      Assert.AreEqual(100, _engine.Info.Nodes);
      Assert.AreEqual("Reset Add Add Add Nodes", log.ToString());
    }

    [TestMethod]
    public void TestInfoSeries()
    {
      PrepareEngine();
      _engine.Go();
      _process.SendOutput("info pv m1 m2 m3 nodes 100");
      _process.SendOutput("info pv m2 m3 depth 200");
      _process.SendOutput("info nodes 300 depth 200");
      _process.SendOutput("info nodes 300 pv 200");
      CollectionAssert.AreEqual(new[] { "200" }, _engine.Info.PrincipalVariation);
      Assert.AreEqual(300, _engine.Info.Nodes);
      Assert.AreEqual(200, _engine.Info.Depth);
      Assert.AreEqual(0, _engine.Info.SelectiveDepth);
      Assert.AreEqual(TimeSpan.Zero, _engine.Info.Time);
    }
    
    [TestMethod]
    public void TestScore()
    {
      PrepareEngine();
      _engine.Go();
      _process.SendOutput("info score cp 100");
      Assert.AreEqual(100, _engine.Info.Score.CentiPawns);
    }

    private void PrepareEngine()
    {
      _engine.Usi();
      _process.SendOutput("usiok");
      _process.InputData.Clear();
    }
  }
}