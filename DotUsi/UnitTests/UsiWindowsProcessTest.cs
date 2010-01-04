using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using DotUsi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
  [TestClass, Ignore]
  public class UsiWindowsProcessTest
  {
    [TestMethod]
    public void BasicTest()
    {
      var process = new UsiWindowsProcess(Path.Combine(Environment.CurrentDirectory, "Echo"));
      var log = new TestLog();
      process.OutputDataReceived += (s, e) => log.Write(e.Line);
      process.WriteLine("test");
      process.WriteLine("quit");
      while (log.ToString() != "test <null>")
      {
        Thread.Sleep(100);
      }
      process.Dispose();
      Assert.AreEqual("test <null>", log.ToString());
      Assert.AreEqual(0, Process.GetProcessesByName("Echo").Length);
    }
    [TestMethod]
    public void QuitWithDelayTest()
    {
      Assert.AreEqual(0, Process.GetProcessesByName("Echo").Length);
      var process = new UsiWindowsProcess(Path.Combine(Environment.CurrentDirectory, "Echo"));
      process.WriteLine("quit with delay");
      process.Dispose();
      Thread.Sleep(200); // Give the system time to refresh that data
      Assert.AreEqual(0, Process.GetProcessesByName("Echo").Length);
    }
    [TestMethod]
    public void DontQuitTest()
    {
      var process = new UsiWindowsProcess(Path.Combine(Environment.CurrentDirectory, "Echo"));
      Assert.AreEqual(1, Process.GetProcessesByName("Echo").Length);
      process.Dispose();
      Thread.Sleep(200); // Give the system time to refresh that data
      Assert.AreEqual(0, Process.GetProcessesByName("Echo").Length);
    }
    [TestMethod]
    public void KillNotificationTest()
    {
      var process = new UsiWindowsProcess(Path.Combine(Environment.CurrentDirectory, "Echo"));
      var log = new TestLog();
      process.OutputDataReceived += (s, e) => log.Write(e.Line);
      Process.GetProcessesByName("Echo")[0].Kill();
      Thread.Sleep(2000); // Give our wrapper time to realize that the process has been killed
      Assert.AreEqual("<null>", log.ToString());
      Assert.IsTrue(process.IsDisposed);
    }
    [TestMethod]
    public void DisposeNotificationTest()
    {
      var process = new UsiWindowsProcess(Path.Combine(Environment.CurrentDirectory, "Echo"));
      var log = new TestLog();
      process.OutputDataReceived += (s, e) => log.Write(e.Line);
      process.Dispose();
      Assert.AreEqual("<null>", log.ToString());
      Assert.IsTrue(process.IsDisposed);
    }
    [TestMethod]
    public void QuitNotificationTest()
    {
      var process = new UsiWindowsProcess(Path.Combine(Environment.CurrentDirectory, "Echo"));
      var log = new TestLog();
      process.OutputDataReceived += (s, e) => log.Write(e.Line);
      process.WriteLine("quit");
      while (log.ToString() != "<null>")
      {
        Thread.Sleep(100);
      }
      Assert.IsTrue(process.IsDisposed);
    }
  }
}