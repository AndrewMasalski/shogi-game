namespace DotUsi
{
  public abstract class UsiSearchModifier
  {
    public string Command { get { return GetCommand(); }}
    protected abstract string GetCommand();
    public override string ToString()
    {
      return GetCommand();
    }
  }
}