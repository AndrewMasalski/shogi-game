namespace DotUsi
{
  /// <summary>Base class for all modifiers USI 'go' command can have</summary>
  public abstract class UsiSearchModifier
  {
    /// <summary>Gets the command text as it's passed to the engine</summary>
    public string Command { get { return GetCommand(); }}
    /// <summary>Override to define what to pass to engine as a part of the 'setoption' command</summary>
    protected abstract string GetCommand();
    /// <summary>Gets the command text as it's passed to the engine</summary>
    public override string ToString()
    {
      return GetCommand();
    }
  }
}