using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Automation;

namespace Yasc.Utils.Automation
{
  public class ApplicationHost
  {
    private Process _process;

    public AutomationElement Open(string path)
    {
      _process = Process.Start(path);
      if (_process == null) throw new Exception("Coudn't have started app");
      int counter = 0;
      while (_process.MainWindowHandle == IntPtr.Zero)
      {
        if (++counter == 600)
        {
          _process.Kill();

          throw new Exception("Process didn't start in 1 min");
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
      throw new Exception("Process didn't quit in 1 min");
    }
  }
}