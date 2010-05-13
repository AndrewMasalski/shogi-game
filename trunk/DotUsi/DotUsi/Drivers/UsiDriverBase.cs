using System;
using Yasc.DotUsi.Process;

namespace Yasc.DotUsi.Drivers
{
  /// <summary>Base class for concrete engine driver</summary>
  public abstract class UsiDriverBase : IUsiProcess
  {
    /// <summary>Gets raw process</summary>
    protected IUsiProcess Process { get; private set; }

    /// <summary>ctor</summary>
    /// <param name="process">Raw process to decorate</param>
    protected UsiDriverBase(IUsiProcess process)
    {
      Process = process;
      Process.OutputDataReceived += OnOutputDataReceived;
    }

    /// <summary>Override to change behaviour</summary>
    protected virtual void OnOutputDataReceived(object sender, LineReceivedEventArgs args)
    {
      OnOutputDataReceived(args);
    }
    /// <summary>Override to change behaviour</summary>
    protected virtual void OnOutputDataReceived(LineReceivedEventArgs args)
    {
      var handler = OutputDataReceivedInternal;
      if (handler != null) handler(this, args);
    }

    /// <summary>Override to change behaviour</summary>
    public virtual void Dispose()
    {
      Process.Dispose();
    }

    /// <summary>Override to change behaviour</summary>
    public virtual void WriteLine(string text)
    {
      Process.WriteLine(text);
    }

    /// <summary>Override to change behaviour</summary>
    protected virtual event EventHandler<LineReceivedEventArgs> OutputDataReceivedInternal;

    /// <summary>Override to change behaviour</summary>
    public virtual event EventHandler<LineReceivedEventArgs> OutputDataReceived
    {
      add { OutputDataReceivedInternal += value; }
      remove { OutputDataReceivedInternal -= value; }
    }
  }
}