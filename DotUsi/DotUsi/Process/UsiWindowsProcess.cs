using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace DotUsi
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
    private readonly Process _process;
    private readonly Timer _timer;

    public UsiWindowsProcess(string path)
    {
     _process = Process.Start(new ProcessStartInfo
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
      if (_process.HasExited)
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

    public void WriteLine(string text)
    {
      File.AppendAllText(@"log.input.txt", text + "\r\n");
      _process.StandardInput.WriteLine(text);
    }

    public event EventHandler<LineReceivedEventArgs> OutputDataReceived;
    public bool IsDisposed { get; private set;}
    public void Dispose()
    {
      // This method could be called simultaneously from 3 threads:
      // 1) User thread
      // 2) Timer thread
      // 3) Console thread <- TODO:
      lock (this)
      {
        if (IsDisposed) return;
        
        _timer.Dispose();
        _process.OutputDataReceived -= OnOutputDataReceived;
        OnOutputDataReceived(new LineReceivedEventArgs(null));
        OutputDataReceived = null;

        if (!_process.WaitForExit(1000))
        {
          _process.CloseMainWindow();
          if (!_process.WaitForExit(1000))
          {
            _process.Kill();
          }
        }
        _process.Dispose();

        IsDisposed = true;
      }
    }
  }
}