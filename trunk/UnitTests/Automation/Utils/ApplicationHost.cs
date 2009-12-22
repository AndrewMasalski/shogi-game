using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yasc;

namespace UnitTests.Automation
{
  public class ApplicationHost
  {
    private Process _process;

    public AutomationElement Open()
    {
      _process = Process.Start(typeof(MainWindow).Assembly.Location);
      if (_process == null) throw new Exception("Coudn't have started app");
      int counter = 0;
      while (_process.MainWindowHandle == IntPtr.Zero)
      {
        if (++counter == 600)
        {
          _process.Kill();
          Assert.Fail("Process didn't start in 1 min");
        }
        Thread.Sleep(100);
        _process.Refresh();
      }
      return AutomationElement.FromHandle(_process.MainWindowHandle);
    }
    public void Close()
    {
      _process.CloseMainWindow();
      if (_process.WaitForExit(60 * 1000)) return;
      {
        _process.Kill();
        Console.WriteLine("We had to kill process on exit");
      }
      Assert.Fail("Process didn't quit in 1 min");
    }
  }
}