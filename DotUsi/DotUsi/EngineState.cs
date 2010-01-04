using System;
using System.Collections.ObjectModel;

namespace DotUsi
{
  public class EngineState
  {
    /// <summary>Search depth</summary>
    public int Depth { get; set; }
    /// <summary>Selective search depth</summary>
    public int SelectiveDepth { get; set; }
    /// <summary>The search time</summary>
    public TimeSpan Time { get; set; }
    /// <summary>Nodes searched</summary>
    public int Nodes { get; set; }
    /// <summary>The best moves sequence found</summary>
    public ReadOnlyCollection<string> PrincipalVariation { get; set; }
  }
}