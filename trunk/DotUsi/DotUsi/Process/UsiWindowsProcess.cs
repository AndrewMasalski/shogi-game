using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using DotUsi.Exceptions;

namespace DotUsi.Process
{
  /// <summary>Default <see cref="IUsiProcess"/> implementation 
  ///   with real windows process in backgroud.</summary>
  /// <remarks><para><see cref="UsiWindowsProcess"/> guarantees 
  ///   that you'll be notified with null line event if 
  ///   process dies because it's been killed, or exited normally,
  ///   or you've called <see cref="Dispose"/></para>
  /// <para><see cref="Dispose"/> guarantees that background 
  ///   process will be dead in a matter of seconds. First it's
  ///   trying to send the process "close" signal, then, if that
  ///   doesn't help, asks system to kill the process.</para>
  /// </remarks>
  public class UsiWindowsProcess : IUsiProcess
  {
    private System.Diagnostics.Process _process;
    private readonly Timer _timer;

    /// <summary>ctor</summary>
    /// <param name="path">Path to the executable file</param>
    public UsiWindowsProcess(string path)
    {
     _process = System.Diagnostics.Process.Start(new ProcessStartInfo
                                 {
                                   FileName = path,
                                   WorkingDirectory = Path.GetDirectoryName(path),
                                   CreateNoWindow = true,
                                   ErrorDialog = false,
                                   RedirectStandardError = true,
                                   RedirectStandardInput = true,
                                   RedirectStandardOutput = true,
                                   UseShellExecute = false,
                                 });
      if (_process == null) throw new CouldntStartProcessException();
      _process.OutputDataReceived += OnOutputDataReceived;
      _process.BeginOutputReadLine();
      _timer = new Timer(OnTimerTick, null, 1000, 1000);
    }

    private void OnTimerTick(object state)
    {
      // TODO: Test for (_process != null) thing?
      if (_process != null && _process.HasExited)
      {
        Dispose();
      }
    }

    private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
    {
      File.AppendAllText(@"log.output.txt", e.Data + "\r\n");
      OnOutputDataReceived(new LineReceivedEventArgs(e.Data));
      if (e.Data == null)
      {
        OutputDataReceived = null;
        Dispose();
      }
    }
    private void OnOutputDataReceived(LineReceivedEventArgs e)
    {
      var handle = OutputDataReceived;
      if (handle != null) handle(this, e);
    }

    /// <summary>Send some input to the process</summary>
    public void WriteLine(string text)
    {
      File.AppendAllText(@"log.input.txt", text + "\r\n");
      if (_process != null) _process.StandardInput.WriteLine(text);
    }

    /// <summary>Raised asynchronously when windows process sends something to its output</summary>
    public event EventHandler<LineReceivedEventArgs> OutputDataReceived;
    /// <summary>Indicates whether the engine is disposed</summary>
    public bool IsDisposed { get { return _process == null; } }
    /// <summary>Closes the windows process. Kills it if it has to.</summary>
    public void Dispose()
    {
      // This method could be called simultaneously from 3 threads:
      // 1) User thread
      // 2) Timer thread
      // 3) Console thread <- TODO:
      lock (this)
      {
        if (_process == null) return;
        var process = _process; _process = null;

        _timer.Dispose();
        process.OutputDataReceived -= OnOutputDataReceived;
        OnOutputDataReceived(new LineReceivedEventArgs(null));
        OutputDataReceived = null;

        if (!process.WaitForExit(1000))
        {
          process.CloseMainWindow();
          if (!process.WaitForExit(1000))
          {
            process.Kill();
          }
        }
        process.Dispose();
      }
    }
  }
}