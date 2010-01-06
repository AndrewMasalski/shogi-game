namespace DotUsi
{
  public abstract class ScalarModifier<T> : UsiSearchModifier
  {
    protected T Value { get; private set; }

    protected ScalarModifier(T value)
    {
      Value = value;
    }

    protected abstract string GetCommandName();
    protected override string GetCommand()
    {
      return GetCommandName() + " " + ValueToString();
    }

    protected virtual string ValueToString()
    {
      return Value.ToString();
    }
  }
}