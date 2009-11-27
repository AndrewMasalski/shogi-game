using System.Collections.Generic;
using Yasc.ShogiCore.Utils;

namespace UnitTests.Diagram
{
  public interface IUsualMoves
  {
    Position From { get; }
    IEnumerable<Position> To { get; }
    IEnumerable<Position> NotTo { get; }
  }
}