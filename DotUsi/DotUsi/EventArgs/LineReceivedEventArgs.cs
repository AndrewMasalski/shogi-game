using System;

namespace DotUsi
{
  /// <summary>Contains a line of console output</summary>
  public class LineReceivedEventArgs : EventArgs
  {
    /// <summary>Line of console output</summary>
    public string Line { get; private set; }
    
    /// <summary>ctor</summary>
    internal LineReceivedEventArgs(string line)
    {
      Line = line;
    }
  }
}