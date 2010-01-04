using System;
using System.Collections.Generic;
using DotUsi;

namespace UnitTests
{
  public class TestProcess : IUsiProcess
  {
    void IDisposable.Dispose()
    {
      // Do nothing
    }

    public void SendOutput(string text)
    {
      var received = OutputDataReceived;
      if (received != null) received(this, new LineReceivedEventArgs(text));
    }

//    public event Action<string> ProcessInput;

    void IUsiProcess.WriteLine(string text)
    {
      InputData.Enqueue(text);
//      var action = ProcessInput;
//      if (action != null) action(text);
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

/*



 
 */