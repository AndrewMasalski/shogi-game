using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.DotUsi;
using Yasc.DotUsi.Process;

namespace DotUsi.UnitTests
{
  [TestClass]
  public class UsiEngineAsynchOperationsTest
  {
    private UsiWindowsProcess _process;
    private UsiEngine _engine;

    [TestInitialize]
    public void Init()
    {
      _process = new UsiWindowsProcess("Echo");
      _engine = new UsiEngine(_process);
    }
    [TestCleanup]
    public void Release()
    {
      _process.Dispose();
    }

    [TestMethod]
    public void UsiIsAsynchTest()
    {
      var evnt = new AutoResetEvent(false);
      _engine.UsiOK += (s, e) => evnt.Set();
      _engine.Usi();
      // If Usi were synch we'd never get here
      _process.WriteLine("usiok");
      evnt.WaitOne();
      // If Usi didn't recognize usiok async we'd have Usi mode here
      Assert.AreEqual(EngineMode.Ready, _engine.Mode);
    }

    [TestMethod]
    public void SynchUsiTest()
    {
      bool ok = false;
      using (new Timer(s => { _process.WriteLine("usiok"); ok = true; }, null, 500, 0))
      {
        _engine.SynchUsi();
      }
      Assert.IsTrue(ok);
    }
  }
}