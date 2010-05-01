using System.Threading;
using DotUsi.Process;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc.Utils;

namespace DotUsi.UnitTests
{
  [TestClass]
  public class UsiWindowsProcessTest
  {
    private UsiWindowsProcess _process;
    private TestLog _log;

    [TestInitialize]
    public void Init()
    {
      Assert.AreEqual(0, System.Diagnostics.Process.GetProcessesByName("Echo").Length);
      _process = new UsiWindowsProcess("Echo");
      _log = new TestLog();
    }
    [TestMethod]
    public void BasicTest()
    {
      _process.OutputDataReceived += (s, e) => _log.Write(e.Line);
      _process.WriteLine("test");
      _process.WriteLine("quit");
      while (_log.ToString() != "test <null>")
      {
        Thread.Sleep(100);
      }
      _process.Dispose();
      Assert.AreEqual("test <null>", _log.ToString());
      Assert.AreEqual(0, System.Diagnostics.Process.GetProcessesByName("Echo").Length);
    }
    [TestMethod]
    public void QuitWithDelayTest()
    {
      _process.WriteLine("quit with delay");
      _process.Dispose();
      Thread.Sleep(200); // Give the system time to refresh that data
      Assert.AreEqual(0, System.Diagnostics.Process.GetProcessesByName("Echo").Length);
    }
    [TestMethod]
    public void DisposeAndDontQuitTest()
    {
      Thread.Sleep(200); // Give the system time to refresh that data
      Assert.AreEqual(1, System.Diagnostics.Process.GetProcessesByName("Echo").Length);
      _process.Dispose();
      Thread.Sleep(200); // Give the system time to refresh that data
      Assert.AreEqual(0, System.Diagnostics.Process.GetProcessesByName("Echo").Length);
    }
    [TestMethod]
    public void KillNotificationTest()
    {
      _process.OutputDataReceived += (s, e) => _log.Write(e.Line);
      System.Diagnostics.Process.GetProcessesByName("Echo")[0].Kill();
      Thread.Sleep(2000); // Give our wrapper time to realize that the process has been killed
      Assert.AreEqual("<null>", _log.ToString());
      Assert.IsTrue(_process.IsDisposed);
    }
    [TestMethod]
    public void DisposeNotificationTest()
    {
      _process.OutputDataReceived += (s, e) => _log.Write(e.Line);
      _process.Dispose();
      Assert.AreEqual("<null>", _log.ToString());
      Assert.IsTrue(_process.IsDisposed);
    }
    [TestMethod]
    public void QuitNotificationTest()
    {
      _process.OutputDataReceived += (s, e) => _log.Write(e.Line);
      _process.WriteLine("quit");
      while (_log.ToString() != "<null>")
      {
        Thread.Sleep(100);
      }
      Assert.IsTrue(_process.IsDisposed);
    }
    [TestMethod]
    public void CallDisposeTwiceTest()
    {
      _process.Dispose();
      _process.Dispose();
    }
    [TestMethod]
    public void CallDisposeInEvent()
    {
      _process.OutputDataReceived += (s, e) => _process.Dispose();
      _process.Dispose();
    }
  }
}