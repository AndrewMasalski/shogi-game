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
    }
    [TestMethod]
    public void TestInfoUpdated()
    {
      Assert.Inconclusive();
    }
    private void PrepareEngine()
    {
      _engine.Usi();
      _process.SendOutput("usiok");
      _process.InputData.Clear();
    }
  }
}