namespace DotUsi
{
  /// <summary><para>Start searching in pondering mode.</para> 
  ///   <para>This means that the last move X sent in the current position is the move to ponder on. 
  ///   The engine can do what it wants to do, but after a <see cref="PonderHit"/> command 
  ///   it should continue with move X. </para>
  /// 
  /// </summary>
  /// <remarks>
  /// <para>This means that the ponder move sent by the GUI can be interpreted as a recommendation about which move to ponder on.
  /// However, if the engine decides to ponder on a different move, it should not display any mainlines as they are likely 
  /// to be misinterpreted by the GUI because the GUI expects the engine to ponder on the suggested move.</para>
  /// <para>Engine won't exit the search in ponder mode, even if it's mate!</para></remarks>
  public class PonderModifier : UsiSearchModifier
  {
    public override string ToString()
    {
      return "ponder";
    }
  }
}