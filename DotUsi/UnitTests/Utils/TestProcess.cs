using System;
using System.Collections.Generic;
using DotUsi.Process;

namespace DotUsi.UnitTests.Utils
{
  public class TestProcess : IUsiProcess
  {
    void IDisposable.Dispose()
    {
      InputData.Enqueue("<TestProcess: Dispose()>");
      SendOutput(null);
    }

    public void SendOutput(string text)
    {
      var received = OutputDataReceived;
      if (received != null) received(this, new LineReceivedEventArgs(text));
    }
    void IUsiProcess.WriteLine(string text)
    {
      InputData.Enqueue(text);
    }

    private event EventHandler<LineReceivedEventArgs> OutputDataReceived;


    public TestProcess()
    {
      InputData = new Queue<string>();
    }

    public Queue<string> InputData { get; private set; }


    event EventHandler<LineReceivedEventArgs> IUsiProcess.OutputDataReceived
    {
      add { OutputDataReceived += value; }
      remove { OutputDataReceived -= value; }
    }
  }
}
