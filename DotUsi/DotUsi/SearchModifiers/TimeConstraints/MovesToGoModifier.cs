namespace DotUsi
{
  /// <summary>There are n moves to the next time control. 
  ///   This will only be sent if n > 0. 
  ///   If you don't get this and get the <see cref="WhiteTimeModifier"/> and <see cref="BlackTimeModifier"/>,
  ///     it's sudden death.
  /// </summary>
  public class MovesToGoModifier : ScalarModifier<int>
  {
    public MovesToGoModifier(int value) 
      : base(value)
    {
    }

    protected override string GetCommandName()
    {
      return "movestogo";
    }
  }
}