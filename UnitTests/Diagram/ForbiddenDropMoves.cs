namespace UnitTests.Diagram
{
  public class ForbiddenDropMoves : DropMovesBase
  {
    public override bool IsAvailable
    {
      get { return false; }
    }
  }
}