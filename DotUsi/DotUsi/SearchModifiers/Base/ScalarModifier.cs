namespace Yasc.DotUsi.SearchModifiers.Base
{
  /// <summary>Base class for all search modifiers with scalar value</summary>
  /// <typeparam name="T">value type</typeparam>
  public abstract class ScalarModifier<T> : UsiSearchModifier
  {
    /// <summary>Modifier parameter</summary>
    protected T Value { get; private set; }

    /// <summary>ctor</summary>
    protected ScalarModifier(T value)
    {
      Value = value;
    }

    /// <summary>Override to define what to pass to engine as a name of option in 'setoption' command</summary>
    protected abstract string GetCommandName();
    /// <summary>Override to define what to pass to engine as a part of the 'setoption' command</summary>
    protected override string GetCommand()
    {
      return GetCommandName() + " " + ValueToString();
    }

    /// <summary>Override to define how to convert <see cref="ScalarModifier{T}.Value"/> to string</summary>
    protected virtual string ValueToString()
    {
      return Value.ToString();
    }
  }
}