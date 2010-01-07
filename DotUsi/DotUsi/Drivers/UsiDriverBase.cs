using System;

namespace DotUsi
{
  public abstract class UsiDriverBase : IUsiProcess
  {
    protected IUsiProcess Process { get; private set; }

    protected UsiDriverBase(IUsiProcess process)
    {
      Process = process;
      Process.OutputDataReceived += OnOutputDataReceived;
    }

    protected virtual void OnOutputDataReceived(object sender, LineReceivedEventArgs args)
    {
      OnOutputDataReceived(args);
    }
    protected virtual void OnOutputDataReceived(LineReceivedEventArgs args)
    {
      var handler = OutputDataReceivedInternal;
      if (handler != null) handler(this, args);
    }

    public virtual void Dispose()
    {
      Process.Dispose();
    }

    public virtual void WriteLine(string text)
    {
      Process.WriteLine(text);
    }

    protected virtual event EventHandler<LineReceivedEventArgs> OutputDataReceivedInternal;

    public virtual event EventHandler<LineReceivedEventArgs> OutputDataReceived
    {
      add { OutputDataReceivedInternal += value; }
      remove { OutputDataReceivedInternal -= value; }
    }
  }
}