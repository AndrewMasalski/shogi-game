using System;

namespace DotUsi.Process
{
  /// <summary>Abstraction for an engine process</summary>
  public interface IUsiProcess : IDisposable
  {
    /// <summary>Send some input to the process</summary>
    void WriteLine(string text);
    /// <summary>Raised when some output is received from process</summary>
    event EventHandler<LineReceivedEventArgs> OutputDataReceived;
  }
}