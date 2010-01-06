using System;

namespace DotUsi
{
  public interface IUsiProcess : IDisposable
  {
    void WriteLine(string text);
    event EventHandler<LineReceivedEventArgs> OutputDataReceived;
  }
}