using System;

namespace DotUsi
{
  public class LineReceivedEventArgs : EventArgs
  {
    public string Line { get; private set; }

    public LineReceivedEventArgs(string line)
    {
      Line = line;
    }
  }
}